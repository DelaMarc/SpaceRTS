using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Modules
{
    public class CameraModule : Module
    {
        [SerializeField] CinemachineFreeLook m_camera;
        [SerializeField] float m_screenXOffset = 0, m_screenYOffset = 0;
        float m_cameraSpeed = 20f;
        public override void Manage()
        {
            base.Manage();
            ManageMouseMovement();
            ManageRotation();
            //m_camera.m_XAxis.Value = Input.GetAxis("Vertical")
        }

        private void ManageRotation()
        {
            if (m_camera.Follow == null)
            {
                return;
            }
            if (Input.GetMouseButtonDown(1))
            {
                m_camera.m_XAxis.m_InputAxisName = "Mouse X";
                m_camera.m_YAxis.m_InputAxisName = "Mouse Y";
            }
            if (Input.GetMouseButtonUp(1))
            {
                m_camera.m_XAxis.m_InputAxisName = null;
                m_camera.m_YAxis.m_InputAxisName = null;
            }
            //m_camera.m_XAxis.Value = Input.GetAxis("Mouse X");
            //m_camera.m_YAxis.Value = Input.GetAxis("Mouse Y");
        }

        private void ManageMouseMovement()
        {
            if (m_camera.Follow != null)
            {
                return;
            }
            //Vector3 cameraPos = m_camera.transform.position;
            Vector3 dir = Vector3.zero;
            if (Input.mousePosition.x <= m_screenXOffset)
            {
                dir += -Camera.main.transform.right;
                //cameraPos += Vector3.left * m_cameraSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x >= Screen.width - m_screenXOffset)
            {
                dir += Camera.main.transform.right;
                //cameraPos += Vector3.right * m_cameraSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y <= m_screenYOffset)
            {
                dir += -Camera.main.transform.forward;
                //cameraPos += Vector3.back * m_cameraSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y >= Screen.height - m_screenYOffset)
            {
                dir += Camera.main.transform.forward;
                //cameraPos += Vector3.forward * m_cameraSpeed * Time.deltaTime;
            }
            dir.y = 0;
            m_camera.transform.position += dir * m_cameraSpeed * Time.deltaTime;
            //m_camera.transform.position = cameraPos;
        }

    }
}