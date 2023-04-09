using Cinemachine;
using Core.Component;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Modules.CameraManagement
{
    public class CameraModule : Module
    {
        private const float TARGET_DISTANCE = 15;
        [SerializeField] float m_maxRotationAngle = 180f;
        [SerializeField] private float m_camFollowSpeed = 0.2f;
        [SerializeField] private float m_cameraLookSpeed = 2f;
        [SerializeField] private float m_cameraPivotSpeed = 2f;
        //private CinemachineFreeLook m_camera;
        private CameraManagement Camera;
        private Vector3 m_camFollowVelocity = Vector3.zero;
        private SelectableComponent m_currentSelected;
        private float m_zoomMax;
        private Vector3 m_zoomDir;
        private int m_zoomCount = 0;
        private float m_timeZoomStart;
        //rotation
        Vector3 m_previousCameraPosition;
        Vector2 m_mousePos;
        private bool m_rotating, m_rotatePressedThisFrame;
        private float m_lookAngle, m_pivotAngle, m_targetDistance;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
            //m_camera = (Entity as CameraManagement).Camera;
            Camera = Entity as CameraManagement;
            m_rotating = false;
            Camera.Actions.General.Select.started += OnTrySelect;
            Camera.Actions.Camera.Zoom.performed += OnZoom;
            Camera.Actions.Camera.Rotate.performed += OnRotatePressed;
            Camera.Actions.Camera.Rotate.canceled += OnRotatePressed;
            //Camera.Actions.Camera.RotateHeld.performed += OnRotateHeld;
            Debug.Log($"<color=green>Camera module started</color>");
        }

        public override void LateManage()
        {
            base.Manage();
            m_mousePos = Entity.Actions.Camera.Point.ReadValue<Vector2>();
            FollowTarget();
            ManageRotation();
            //m_camera.m_XAxis.Value = Input.GetAxis("Vertical")
            m_rotatePressedThisFrame = false;
        }

        private void ManageRotation()
        {
            if (m_currentSelected == null || !m_rotating || m_rotatePressedThisFrame)
            {
                return;
            }
            var cam = Camera.Camera;
            Vector3 currentPosition = cam.ScreenToViewportPoint(m_mousePos);
            Vector3 direction = m_previousCameraPosition - currentPosition;

            float rotationAroundYAxis = -direction.x * m_maxRotationAngle; // camera moves horizontally
            float rotationAroundXAxis = direction.y * m_maxRotationAngle; // camera moves vertically
            cam.transform.position = m_currentSelected.transform.position;
            Debug.Log($"Manage Rotation, around X: {rotationAroundXAxis}, around Y: {rotationAroundXAxis}");

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);

            cam.transform.Translate(new Vector3(0, 0, -m_targetDistance));
            m_previousCameraPosition = currentPosition;

            //return;
            //Vector2 rotationInput = Entity.Actions.Camera.RotationAxis.ReadValue<Vector2>();
            //Vector3 rotation = Vector3.zero;
            //Quaternion targetRotation;

            //m_lookAngle = m_lookAngle + (rotationInput.x * m_cameraLookSpeed);
            //m_pivotAngle = m_pivotAngle + (rotationInput.y * m_cameraPivotSpeed);
            //rotation.y = m_lookAngle;
            //targetRotation = Quaternion.Euler(rotation);
            //Camera.Camera.transform.localRotation = targetRotation;
        }

        private void FollowTarget()
        {
            if (Camera.Target == null)
            {
                return;
            }
            Vector3 dest = Camera.Target.transform.position - m_zoomDir;
            Vector3 pos = Camera.Camera.transform.position;
            float dist = (pos - dest).sqrMagnitude;
            if (dist < 0.01f)
            {
                //m_zoomCount = 0;
                m_camFollowVelocity = Vector3.zero;
                return;
            }
            var newPos = Vector3.SmoothDamp(pos,
                dest,
                ref m_camFollowVelocity,
                m_camFollowSpeed);
            Camera.Camera.transform.position = newPos;
        }

        #region Events
        private void OnTrySelect(InputAction.CallbackContext obj)
        {
            RaycastHit hit;
            Vector2 point = Camera.Actions.Camera.Point.ReadValue<Vector2>();
            Ray ray = Camera.Camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                SelectableComponent selectable = hit.collider.GetComponent<SelectableComponent>();
                if (selectable != null)
                {
                    m_currentSelected = selectable;
                    m_zoomCount = 0;
                    Debug.Log($"Selected object {m_currentSelected.name}");
                }
            }
            else
            {
                if (Camera.Target != null)
                {
                    Debug.Log("De selected object");
                    m_currentSelected = null;
                    Camera.Target = null;
                }
            }
        }

        private void OnZoom(InputAction.CallbackContext obj)
        {
            if (m_currentSelected == null)
            {
                return;
            }
            ++m_zoomCount;
            Camera.Target = m_currentSelected.transform;
            if (m_zoomCount == 1)
            {
                m_zoomMax = Vector3.Distance(Camera.Target.position, Camera.Camera.transform.position);
            }
            else
            {
                m_zoomMax = m_currentSelected.MinZoomValue;
            }
            m_camFollowVelocity = Vector3.zero;
            m_timeZoomStart = Time.time;
            m_zoomDir = Camera.Camera.transform.forward * m_zoomMax;
            Debug.Log($"Zoom on {Camera.Target.name}");
        }

        private void OnRotatePressed(InputAction.CallbackContext obj)
        {
            m_rotating = obj.performed;
            if (m_rotating && m_currentSelected != null)
            {
                Camera.Target = null;
                m_zoomCount = 0;
                m_previousCameraPosition = Camera.Camera.ScreenToViewportPoint(m_mousePos);
                m_rotatePressedThisFrame = true;
                m_targetDistance = Vector3.Distance(Camera.Camera.transform.position, m_currentSelected.transform.position);
            }
        }
        #endregion

        private void OnDrawGizmos()
        {
            if (m_currentSelected != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(Camera.Camera.transform.position, m_currentSelected.transform.position);
            }
        }

    }
}