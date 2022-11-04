using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Module : MonoBehaviour
    {
        protected Entity Entity;
        public virtual void Init(Entity a_entity)
        {
            Entity = a_entity;
        }

        public virtual void Manage()
        {

        }

        public virtual void Cleanup()
        {

        }
    }
}