using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Component
{
    public class SelectableComponent : CoreComponent
    {
        [SerializeField] private float m_bottomRigHeight = 2;
        [SerializeField] private float m_bottomRigRadius = 1.3f;

        #region Getters
        public float BottomRigHeight => m_bottomRigHeight;
        public float BottomRigRadius => m_bottomRigRadius;
        #endregion
    }
}