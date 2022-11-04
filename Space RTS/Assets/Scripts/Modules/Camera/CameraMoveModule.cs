using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Modules.CameraManagement
{
    public class CameraMoveModule : Module
    {
        [SerializeField] private float m_cameraSpeed = 20f;
        [SerializeField] private float m_screenXOffset = 0, m_screenYOffset = 0;
        private CinemachineFreeLook m_camera;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
            m_camera = (Entity as CameraManagement).Camera;
        }

        public override void Manage()
        {
            base.Manage();
            ManageMouseMovement();
        }

        private void ManageMouseMovement()
        {
            if (m_camera.Follow != null)
            {
                return;
            }
            Vector3 dir = Vector3.zero;
            if (Input.mousePosition.x <= m_screenXOffset)
            {
                dir += -Camera.main.transform.right;
            }
            if (Input.mousePosition.x >= Screen.width - m_screenXOffset)
            {
                dir += Camera.main.transform.right;
            }
            if (Input.mousePosition.y <= m_screenYOffset)
            {
                dir += -Camera.main.transform.forward;
            }
            if (Input.mousePosition.y >= Screen.height - m_screenYOffset)
            {
                dir += Camera.main.transform.forward;
            }
            dir.y = 0;
            m_camera.transform.position += dir * m_cameraSpeed * Time.deltaTime;
        }
    }
}