using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Modules.CameraManagement
{
    public class CameraManagement : Entity
    {
        [SerializeField] private CinemachineFreeLook m_camera;

        #region Getters
        public CinemachineFreeLook Camera => m_camera;
        #endregion
    }
}