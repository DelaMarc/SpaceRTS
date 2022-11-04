using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Modules.CameraManagement
{
    public class CameraManagement : Entity
    {
        [SerializeField] private Camera m_camera;

        private Transform m_target;

        #region Properties
        public Transform Target
        {
            get => m_target;
            set
            {
                m_target = value;
            }
        }
        #endregion

        #region Getters
        public Camera Camera => m_camera;
        #endregion
    }
}