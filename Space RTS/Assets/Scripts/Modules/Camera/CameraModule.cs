using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Modules.CameraManagement
{
    public class CameraModule : Module
    {
        private CinemachineFreeLook m_camera;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
            m_camera = (Entity as CameraManagement).Camera;
        }

        public override void Manage()
        {
            base.Manage();
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



    }
}