//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Input/RTSInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @RTSInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @RTSInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""RTSInputActions"",
    ""maps"": [
        {
            ""name"": ""Camera"",
            ""id"": ""e764c9f3-7f3b-4f6d-a533-f57a3657f92b"",
            ""actions"": [
                {
                    ""name"": ""RotationAxis"",
                    ""type"": ""PassThrough"",
                    ""id"": ""60fef2c9-f6fa-4df5-b02a-007a037128a5"",
                    ""expectedControlType"": ""Delta"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Button"",
                    ""id"": ""7a8be7a0-13f0-4c3d-acb8-a64e64537e79"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Button"",
                    ""id"": ""636e41b0-fc9e-4d9b-b116-c55b19529539"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""71f3dad1-a032-4f5c-87a6-f46225101066"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""fe9c3ffa-e918-4caf-9b03-2cf50550b47a"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""RotationAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""81df483c-579a-4c9f-9a02-abd950659109"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b7bd8e1-dea3-4568-8654-d2d5d00b4788"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93efb210-99c2-4ce6-b3d6-5e883c91cafd"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""General"",
            ""id"": ""bb4a2550-b898-4e44-b4bb-6f3f9c089b97"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""f7378dff-a7c6-4a37-9df7-41051ae8617f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""14ee0475-3570-4f1e-8def-b105a9cdaf18"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and mouse"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and mouse"",
            ""bindingGroup"": ""Keyboard and mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Camera
        m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
        m_Camera_RotationAxis = m_Camera.FindAction("RotationAxis", throwIfNotFound: true);
        m_Camera_Zoom = m_Camera.FindAction("Zoom", throwIfNotFound: true);
        m_Camera_Rotate = m_Camera.FindAction("Rotate", throwIfNotFound: true);
        m_Camera_Point = m_Camera.FindAction("Point", throwIfNotFound: true);
        // General
        m_General = asset.FindActionMap("General", throwIfNotFound: true);
        m_General_Select = m_General.FindAction("Select", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Camera
    private readonly InputActionMap m_Camera;
    private ICameraActions m_CameraActionsCallbackInterface;
    private readonly InputAction m_Camera_RotationAxis;
    private readonly InputAction m_Camera_Zoom;
    private readonly InputAction m_Camera_Rotate;
    private readonly InputAction m_Camera_Point;
    public struct CameraActions
    {
        private @RTSInputActions m_Wrapper;
        public CameraActions(@RTSInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @RotationAxis => m_Wrapper.m_Camera_RotationAxis;
        public InputAction @Zoom => m_Wrapper.m_Camera_Zoom;
        public InputAction @Rotate => m_Wrapper.m_Camera_Rotate;
        public InputAction @Point => m_Wrapper.m_Camera_Point;
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
        public void SetCallbacks(ICameraActions instance)
        {
            if (m_Wrapper.m_CameraActionsCallbackInterface != null)
            {
                @RotationAxis.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotationAxis;
                @RotationAxis.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotationAxis;
                @RotationAxis.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotationAxis;
                @Zoom.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                @Zoom.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                @Zoom.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                @Rotate.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnRotate;
                @Point.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPoint;
                @Point.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPoint;
                @Point.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPoint;
            }
            m_Wrapper.m_CameraActionsCallbackInterface = instance;
            if (instance != null)
            {
                @RotationAxis.started += instance.OnRotationAxis;
                @RotationAxis.performed += instance.OnRotationAxis;
                @RotationAxis.canceled += instance.OnRotationAxis;
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @Point.started += instance.OnPoint;
                @Point.performed += instance.OnPoint;
                @Point.canceled += instance.OnPoint;
            }
        }
    }
    public CameraActions @Camera => new CameraActions(this);

    // General
    private readonly InputActionMap m_General;
    private IGeneralActions m_GeneralActionsCallbackInterface;
    private readonly InputAction m_General_Select;
    public struct GeneralActions
    {
        private @RTSInputActions m_Wrapper;
        public GeneralActions(@RTSInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_General_Select;
        public InputActionMap Get() { return m_Wrapper.m_General; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GeneralActions set) { return set.Get(); }
        public void SetCallbacks(IGeneralActions instance)
        {
            if (m_Wrapper.m_GeneralActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_GeneralActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_GeneralActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_GeneralActionsCallbackInterface.OnSelect;
            }
            m_Wrapper.m_GeneralActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
            }
        }
    }
    public GeneralActions @General => new GeneralActions(this);
    private int m_KeyboardandmouseSchemeIndex = -1;
    public InputControlScheme KeyboardandmouseScheme
    {
        get
        {
            if (m_KeyboardandmouseSchemeIndex == -1) m_KeyboardandmouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and mouse");
            return asset.controlSchemes[m_KeyboardandmouseSchemeIndex];
        }
    }
    public interface ICameraActions
    {
        void OnRotationAxis(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
    }
    public interface IGeneralActions
    {
        void OnSelect(InputAction.CallbackContext context);
    }
}
