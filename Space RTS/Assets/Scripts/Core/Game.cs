using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Game : MonoBehaviour
    {
        [SerializeField] List<Entity> m_entities;
        private RTSInputActions m_actions;

        private void Awake()
        {
            m_actions = new RTSInputActions();
            m_actions.Enable();
            foreach (Entity entity in m_entities)
            {
                entity.Init(m_actions);
            }
        }

        private void Update()
        {
            foreach (Entity entity in m_entities)
            {
                entity.Manage();
            }
        }

        private void LateUpdate()
        {
            foreach (Entity entity in m_entities)
            {
                entity.LateManage();
            }
        }
    }
}