using Cinemachine;
using Core.Component;
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
        public SelectableComponent CurrentSelected { get; private set; }

        public Transform Target
        {
            get => m_target;
            set
            {
                m_target = value;
            }
        }

        public int ZoomCount { get; private set; } = 0;
        #endregion

        #region Getters
        public Camera Camera => m_camera;
        #endregion

        public void SetCurrentSelected(SelectableComponent a_selected)
        {
            CurrentSelected = a_selected;
        }

        public void IncreaseZoomCount()
        {
            ++ZoomCount;
        }

        public void SetZoomCount(int a_value)
        {
            ZoomCount = a_value;
        }
    }
}