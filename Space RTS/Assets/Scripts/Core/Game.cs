using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Game : MonoBehaviour
    {
        [SerializeField] List<Entity> m_entities;

        private void Awake()
        {
            foreach (Entity entity in m_entities)
            {
                entity.Init();
            }
        }

        private void Update()
        {
            foreach (Entity entity in m_entities)
            {
                entity.Manage();
            }
        }
    }
}