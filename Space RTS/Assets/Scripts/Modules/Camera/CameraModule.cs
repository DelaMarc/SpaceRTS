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
        [SerializeField] float m_zoom = 0f;
        [SerializeField] private float m_camFollowSpeed = 0.2f;
        //private CinemachineFreeLook m_camera;
        private CameraManagement Camera;
        private Vector3 m_camFollowVelocity = Vector3.zero;
        private SelectableComponent m_currentSelected;
        private float m_zoomMax;
        private Vector3 m_zoomDir;
        private int m_zoomCount = 0;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
            //m_camera = (Entity as CameraManagement).Camera;
            Camera = Entity as CameraManagement;
            Camera.Actions.General.Select.started += OnTrySelect;
            Camera.Actions.Camera.Zoom.performed += OnZoom;
            Debug.Log($"<color=green>Camera module started</color>");
        }

        public override void LateManage()
        {
            base.Manage();
            ManageRotation();
            FollowTarget();
            //m_camera.m_XAxis.Value = Input.GetAxis("Vertical")
        }

        private void ManageRotation()
        {
            if (Camera.Target == null)
            {
                return;
            }
            if (Input.GetMouseButtonDown(1))
            {
                //m_camera.m_XAxis.m_InputAxisName = "Mouse X";
                //m_camera.m_YAxis.m_InputAxisName = "Mouse Y";

            }
            if (Input.GetMouseButtonUp(1))
            {
                //m_camera.m_XAxis.m_InputAxisName = null;
                //m_camera.m_YAxis.m_InputAxisName = null;
            }
            //m_camera.m_XAxis.Value = Input.GetAxis("Mouse X");
            //m_camera.m_YAxis.Value = Input.GetAxis("Mouse Y");
        }

        private void FollowTarget()
        {
            if (Camera.Target == null)
            {
                return;
            }
            var newPos = Vector3.SmoothDamp(Camera.Camera.transform.position,
                Camera.Target.transform.position - m_zoomDir,
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
            m_zoomDir = Camera.Camera.transform.forward * m_zoomMax;
            Debug.Log($"Zoom on {Camera.Target.name}");
        }
        #endregion

    }
}