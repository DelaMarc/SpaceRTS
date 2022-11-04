using Cinemachine;
using Core.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Modules
{
    public class SelectionModule : Module
    {
        private Transform m_currentSelected;
        [SerializeField] CinemachineFreeLook m_camera;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
        }

        public override void Manage()
        {
            base.Manage();
            if (Input.GetMouseButtonDown(0))
            {
                TrySelect();
            }
            if (Input.GetMouseButtonDown(2))
            {
                CenterViewOnSelected();
            }
        }

        void CenterViewOnSelected()
        {
            if (m_currentSelected == null)
            {
                return;
            }
            m_camera.Follow = m_currentSelected;
            m_camera.LookAt = m_currentSelected;
        }

        private void TrySelect()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                SelectableComponent selectable = hit.collider.GetComponent<SelectableComponent>();
                if (selectable != null)
                {
                    Debug.Log("Selected object");
                    m_currentSelected = selectable.transform;
                }
            }
            else
            {
                if (m_currentSelected != null)
                {
                    Debug.Log("De selected object");
                    m_currentSelected = null;
                    m_camera.Follow = null;
                    m_camera.LookAt = null;
                }
            }
        }
    }
}