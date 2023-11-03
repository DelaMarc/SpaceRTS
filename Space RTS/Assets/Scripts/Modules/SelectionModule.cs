using Core.Component;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Modules.CameraManagement
{
    public class SelectionModule : Module
    {
        private CameraManagement Camera;

        public override void Init(Entity a_entity)
        {
            base.Init(a_entity);
            Debug.Assert(Entity != null, "Entity is null");
            Debug.Log($"Init selection module, entity name {Entity.name}");
            Camera = Entity as CameraManagement;
            Camera.Actions.General.Select.started += OnTrySelect;
            Debug.Log($"<color=green>Camera module started</color>");
        }

        public override void Cleanup()
        {
            base.Cleanup();
            Camera.Actions.General.Select.started -= OnTrySelect;
        }

        private void OnTrySelect(InputAction.CallbackContext obj)
        {
            RaycastHit hit;
            Vector2 point = Camera.Actions.Camera.Point.ReadValue<Vector2>();
            Ray ray = Camera.Camera.ScreenPointToRay(point);

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Camera.CurrentSelected?.Deselect();
                SelectableComponent selectable = hit.collider.GetComponent<SelectableComponent>();
                if (selectable != null)
                {
                    selectable.Select();
                    Camera.SetCurrentSelected(selectable);
                    Camera.SetZoomCount(0);
                }
            }
            else
            {
                Camera.CurrentSelected?.Deselect();
                if (Camera.Target != null)
                {
                    Camera.SetCurrentSelected(null);
                }
            }
        }
    }
}