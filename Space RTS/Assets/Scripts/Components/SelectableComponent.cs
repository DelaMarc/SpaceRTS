using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Component
{
    public class SelectableComponent : CoreComponent
    {
        [SerializeField] private float m_minZoomValue = 2.5f;

        #region Getters
        public float MinZoomValue => m_minZoomValue;
        #endregion
    }
}