using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Component
{
    public class SelectableComponent : CoreComponent
    {
        [SerializeField] private float m_minZoomValue = 2.5f;
        [SerializeField] private Renderer m_renderer;
        private Color m_defaultColor;

        #region Getters
        internal int OwnerId { get; private set; }
        public float MinZoomValue => m_minZoomValue;
        #endregion

        private void Awake()
        {
            m_defaultColor = m_renderer.material.color;   
        }

        public void Select()
        {
            m_renderer.material.color = Color.green;
        }

        public void Deselect()
        {
            m_renderer.material.color = m_defaultColor;
        }
    }
}