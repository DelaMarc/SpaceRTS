using Cinemachine;
using Core.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Modules
{
    public class SelectionModule : Module
    {
        private const int BOTTOM_RIG = 2;
        [SerializeField] CinemachineFreeLook m_camera;

        private SelectableComponent m_currentSelected;
        private int m_zoomCount;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
            m_zoomCount = 0;
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
            ++m_zoomCount;
            Debug.Log($"Zoom Count : {m_zoomCount}");
            if (m_zoomCount == 1)
            {
                m_camera.m_YAxisRecentering.RecenterNow();
                m_camera.m_RecenterToTargetHeading.RecenterNow();
                //float dist = Vector3.Distance(m_currentSelected.transform.position, m_camera.transform.position);
                //for (int i = 0; i < m_camera.m_Orbits.Length; ++i)
                //{
                //    m_camera.m_Orbits[i].m_Radius *= dist;
                //    m_camera.m_Orbits[i].m_Height *= dist;
                //}
                
                //m_camera.m_Orbits[BOTTOM_RIG].m_Radius = dist;
                //m_camera.m_Orbits[BOTTOM_RIG].m_Height = dist;
            }
            else
            {
                m_camera.m_Orbits[BOTTOM_RIG].m_Radius = m_currentSelected.BottomRigRadius;
                m_camera.m_Orbits[BOTTOM_RIG].m_Height = m_currentSelected.BottomRigHeight;
            }
            m_camera.Follow = m_currentSelected.transform;
            m_camera.LookAt = m_currentSelected.transform;
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
                    m_currentSelected = selectable;
                    m_zoomCount = 0;
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
                    m_zoomCount = 0;
                }
            }
        }
    }
}