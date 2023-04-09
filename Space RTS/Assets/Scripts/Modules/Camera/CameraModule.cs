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
        //rotation
        private bool m_rotating;
        private float m_lookAngle, m_pivotAngle, m_targetDistance;
        float m_rotationX;
        float m_rotationY;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
            Camera = Entity as CameraManagement;
            m_rotating = false;
            Camera.Actions.General.Select.started += OnTrySelect;
            Camera.Actions.Camera.Zoom.performed += OnZoom;
            Camera.Actions.Camera.Rotate.performed += OnRotatePressed;
            Camera.Actions.Camera.Rotate.canceled += OnRotatePressed;
            Debug.Log($"<color=green>Camera module started</color>");
            m_rotationX = 0;
            m_rotationY = 0;
        }

        public override void LateManage()
        {
            base.Manage();
            FollowTarget();
            ManageRotation();
        }

        private void ManageRotation()
        {
            if (!m_rotating)
            {
                return;
            }
            var input = Entity.Actions.Camera.RotationAxis.ReadValue<Vector2>();
            // no target selected, the camera rotates around itself
            if (m_currentSelected == null)
            {
                Vector3 rot = Vector3.zero;
                Quaternion targetRotation;

                m_lookAngle = m_lookAngle + (input.x * m_cameraLookSpeed * Time.fixedDeltaTime);
                m_pivotAngle = Mathf.Clamp(m_pivotAngle + (input.y * m_cameraPivotSpeed * Time.fixedDeltaTime), -90f, 90f);
                Debug.Log($"{m_lookAngle} {m_pivotAngle}");
                rot.y = m_lookAngle;
                rot.x = -m_pivotAngle;
                targetRotation = Quaternion.Euler(rot);
                Camera.Camera.transform.localRotation = targetRotation;
                return;
            }
            // camera orbits around target
            var cam = Camera.Camera;
            Debug.Log(input);
            m_rotationY += input.x * m_cameraLookSpeed * Time.fixedDeltaTime;
            m_rotationX -= input.y * m_cameraPivotSpeed * Time.fixedDeltaTime;
            m_rotationX = Mathf.Clamp(m_rotationX, -90, 90);
            Quaternion rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0);
            // Calcule la position de la caméra en fonction de la rotation et de la distance par rapport à l'objet cible
            Vector3 position = rotation * new Vector3(0, 0, -m_targetDistance) + m_currentSelected.transform.position;
            // Applique la rotation et la position de la caméra
            cam.transform.transform.rotation = rotation;
            cam.transform.transform.position = position;            
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