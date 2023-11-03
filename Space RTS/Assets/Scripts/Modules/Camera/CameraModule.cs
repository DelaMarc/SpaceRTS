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
        private const float MAX_VERTICAL_ANGLE = 90;
        [SerializeField] float m_maxRotationAngle = 180f;
        [SerializeField] private float m_camFollowSpeed = 0.2f;
        [SerializeField] private float m_cameraLookSpeed = 2f;
        [SerializeField] private float m_cameraPivotSpeed = 2f;
        [Header("Zoom")]
        [SerializeField] private float m_minZoomDist = 3;
        [SerializeField] private float m_maxZoomDist = 10;
        [SerializeField] private float m_zoomOutSpeed;
        [SerializeField] private float m_timeReachMinZoom = 1;
        [SerializeField] private float m_timeReachMaxZoom = 1;
        private float m_zoomInTimer = 0;
        private float m_zoomOutTimer = 0;
        //private CinemachineFreeLook m_camera;
        private CameraManagement Camera;
        private Vector3 m_camFollowVelocity = Vector3.zero;
        //private SelectableComponent m_currentSelected;
        private float m_zoomMax;
        private Vector3 m_zoomDir;
        //private int m_zoomCount = 0;
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
            Camera.Actions.Camera.Focus.performed += OnFocus;
            Camera.Actions.Camera.Rotate.performed += OnRotatePressed;
            Camera.Actions.Camera.Rotate.canceled += OnRotatePressed;
            Debug.Log($"<color=green>Camera module started</color>");
            m_rotationX = 0;
            m_rotationY = 0;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Camera.Actions.Camera.Focus.performed -= OnFocus;
            Camera.Actions.Camera.Rotate.performed -= OnRotatePressed;
            Camera.Actions.Camera.Rotate.canceled -= OnRotatePressed;
        }

        public override void LateManage()
        {
            base.Manage();
            FollowTarget();
            ManageRotation();
            ManageZoom();
        }

        private void ManageZoom()
        {
            if (Camera.Target == null || m_rotating)
            {
                m_zoomOutTimer = 0;
                m_zoomInTimer = 0;
                return;
            }
            float zoomValue = Camera.Actions.Camera.Zoom.ReadValue<float>();
            if (zoomValue != 0f)
            {
                float minZoom = Camera.CurrentSelected.MinZoomValue;
                float currentZoom = (Camera.Camera.transform.position - Camera.Target.position).magnitude;
                // Zoom Out
                if (zoomValue < 0f)
                {
                    m_zoomInTimer = 0;
                    currentZoom = Mathf.Lerp(currentZoom, m_maxZoomDist, m_zoomOutTimer / m_timeReachMaxZoom);
                    m_zoomOutTimer = Mathf.Min(m_zoomOutTimer + Time.fixedDeltaTime, m_timeReachMaxZoom);
                }
                // Zoom In
                else
                {
                    m_zoomOutTimer = 0;
                    currentZoom = Mathf.Lerp(currentZoom, minZoom, m_zoomInTimer / m_timeReachMinZoom);
                    m_zoomInTimer = Mathf.Min(m_zoomInTimer + Time.fixedDeltaTime, m_timeReachMinZoom);
                }
                m_zoomDir = Camera.Camera.transform.forward * currentZoom;
            }
        }

        private void ManageRotation()
        {
            if (!m_rotating)
            {
                return;
            }
            var input = Entity.Actions.Camera.RotationAxis.ReadValue<Vector2>();
            // no target selected, the camera rotates around itself
            if (Camera.Target == null)
            {
                Vector3 rot = Vector3.zero;
                Quaternion targetRotation;

                m_lookAngle = m_lookAngle + (input.x * m_cameraLookSpeed * Time.fixedDeltaTime);
                m_pivotAngle = Mathf.Clamp(m_pivotAngle + (input.y * m_cameraPivotSpeed * Time.fixedDeltaTime), -90f, 90f);
                rot.y = m_lookAngle;
                rot.x = -m_pivotAngle;
                targetRotation = Quaternion.Euler(rot);
                Camera.Camera.transform.localRotation = targetRotation;
                return;
            }
            // camera orbits around target
            var cam = Camera.Camera;
            m_rotationY += input.x * m_cameraLookSpeed * Time.fixedDeltaTime;
            m_rotationX -= input.y * m_cameraPivotSpeed * Time.fixedDeltaTime;
            m_rotationX = Mathf.Clamp(m_rotationX, -MAX_VERTICAL_ANGLE, MAX_VERTICAL_ANGLE);
            Quaternion rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0);
            // Calcule la position de la caméra en fonction de la rotation et de la distance par rapport à l'objet cible
            Vector3 position = rotation * new Vector3(0, 0, -m_targetDistance) + Camera.Target.transform.position;
            // Applique la rotation et la position de la caméra
            cam.transform.transform.rotation = rotation;
            cam.transform.transform.position = position;
        }

        private void FollowTarget()
        {
            if (Camera.Target == null || m_rotating)
            {
                return;
            }
            Vector3 dest = Camera.Target.transform.position - m_zoomDir;
            float distance = Vector3.Distance(dest, Camera.Camera.transform.position);
            float refDist = Camera.ZoomCount == 1 ? 0.01f : m_zoomMax;
            //smooth follow target
            if (distance > refDist)
            {
                Vector3 pos = Camera.Camera.transform.position;
                float dist = (pos - dest).sqrMagnitude;
                if (dist < 0.01f)
                {
                    m_camFollowVelocity = Vector3.zero;
                    return;
                }
                var newPos = Vector3.SmoothDamp(pos,
                    dest,
                    ref m_camFollowVelocity,
                    m_camFollowSpeed);
                Camera.Camera.transform.position = newPos;
            }
            //lock camera to target if we are close enough
            else
            {
                Camera.Camera.transform.position = dest;
            }
            //Debug.Log($"Follow dist from dest point {(Camera.Camera.transform.position - dest).sqrMagnitude} | {m_zoomMax}");
            Debug.DrawLine(Camera.Camera.transform.position, dest, Color.black);
        }

        #region Events

        private void OnFocus(InputAction.CallbackContext obj)
        {
            if (Camera.CurrentSelected == null)
            {
                return;
            }
            Camera.IncreaseZoomCount();
            Camera.Target = Camera.CurrentSelected.transform;
            if (Camera.ZoomCount == 1)
            {
                m_zoomMax = Vector3.Distance(Camera.Target.position, Camera.Camera.transform.position);
            }
            else
            {
                m_zoomMax = Camera.CurrentSelected.MinZoomValue;
            }
            m_camFollowVelocity = Vector3.zero;
            m_zoomDir = Camera.Camera.transform.forward * m_zoomMax;
            Debug.Log($"Zoom on {Camera.Target.name}");
        }

        private void OnRotatePressed(InputAction.CallbackContext obj)
        {
            m_rotating = obj.performed;
            if (m_rotating && Camera.CurrentSelected != null)
            {
                m_targetDistance = Vector3.Distance(Camera.Camera.transform.position, Camera.CurrentSelected.transform.position);
                Camera.Target = Camera.CurrentSelected?.transform;
            }
            // Rotation released, set zoom settings to first state
            else
            {
                m_zoomMax = Vector3.Distance(Camera.Target.position, Camera.Camera.transform.position);
                m_zoomDir = Camera.Camera.transform.forward * m_zoomMax;
                Camera.SetZoomCount(1);
            }
        }
        #endregion

    }
}