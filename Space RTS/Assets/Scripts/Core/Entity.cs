﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] List<Module> m_modules;
        private RTSInputActions m_actions;

        #region Properties
        public RTSInputActions Actions => m_actions;
        #endregion

        public virtual void Init(RTSInputActions a_actions)
        {
            m_actions = a_actions;
            foreach (Module module in m_modules)
            {
                module.Init(this);
            }
        }

        public virtual void Manage()
        {
            foreach (Module module in m_modules)
            {
                module.Manage();
            }
        }

        public virtual void LateManage()
        {
            foreach (Module module in m_modules)
            {
                module.LateManage();
            }
        }

        public virtual void Cleanup()
        {
            foreach (Module module in m_modules)
            {
                module.Cleanup();
            }
        }

        public void RegisterModule(Module a_module)
        {
            if (m_modules.Contains(a_module))
            {
                return;
            }
            m_modules.Add(a_module);
            a_module.Init(this);
        }

        public void UnregisterModule(Module a_module)
        {
            if (!m_modules.Contains(a_module))
            {
                return;
            }
            m_modules.Remove(a_module);
            a_module.Cleanup();
        }
    }
}