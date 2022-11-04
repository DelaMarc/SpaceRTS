using Cinemachine;
using Core.Component;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRecenter : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera m_camera;
    private SelectableComponent m_currentSelected;
    private int m_zoomCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelect();
        }
        if (Input.GetMouseButtonDown(2))
        {
            CenterViewOnSelected();
        }

    }

    private void CenterViewOnSelected()
    {
        m_camera.Follow = m_currentSelected.transform;
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
                //Array.Copy(m_camera.m_Orbits, m_savedOrbits, m_savedOrbits.Length);
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
