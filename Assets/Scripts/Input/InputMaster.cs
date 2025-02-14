// GENERATED AUTOMATICALLY FROM 'Assets/Resources/InputActions/InputActions_Main.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Flamingo
{
    public class InputMaster : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public InputMaster()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions_Main"",
    ""maps"": [
        {
            ""name"": ""Player_Mateo"",
            ""id"": ""cb325c07-1816-489f-8b4b-6c71af8ec295"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""2ee33b5c-888b-41ca-9ab3-6363c0607c7e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FireShooting_Frontal"",
                    ""type"": ""Button"",
                    ""id"": ""66caf3e6-41b0-4d69-b67b-0840ac28e3b6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack_Sword"",
                    ""type"": ""Button"",
                    ""id"": ""1144f27a-1f3d-47a8-befd-0eb3d9f57d91"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftAxes"",
                    ""type"": ""Button"",
                    ""id"": ""ac584050-0443-40c1-a687-46ad064b09dc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightAxes"",
                    ""type"": ""Button"",
                    ""id"": ""126aa351-12c3-4945-904d-7d84815aae03"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bf3f1ae4-4613-4512-9d66-03c4a07e0922"",
                    ""path"": ""<NPad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba28b3d3-a3f9-4ad6-b1f3-a16f8dab2c48"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_Xbox360"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23a03160-66db-4b45-9d3f-62a5dd9186e9"",
                    ""path"": ""<NPad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""FireShooting_Frontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2969ed1a-08f0-4d68-94df-5fe4c622a9b4"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_Xbox360"",
                    ""action"": ""FireShooting_Frontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c95dfe25-8a28-4791-919b-a52e89ad417f"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_Xbox360"",
                    ""action"": ""Attack_Sword"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""da969260-53b4-4148-a787-f4cfdac60e18"",
                    ""path"": ""<NPad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""Attack_Sword"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""595d6579-c029-485c-8a62-58b740ed661e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6799680c-2e9f-49c9-b419-d849820469cd"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_Xbox360"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bc224ace-6e6d-4299-96e2-e717dad21213"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_Xbox360"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c094b166-edb6-49c2-b564-501cccb40852"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_Xbox360"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e7d360c9-f31a-4ead-864c-cfe2d8faaf03"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_Xbox360"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2DVector"",
                    ""id"": ""b8c8d6c6-2489-4f72-821c-1cf3e9a3ca14"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""08e76c3c-3e4e-42c1-9747-b223cdeea6f4"",
                    ""path"": ""<NPad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ca275f95-3a1a-4ca2-91f6-bb228ddc80d7"",
                    ""path"": ""<NPad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""91f687ef-9c9c-495f-aa6a-7504be303985"",
                    ""path"": ""<NPad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d98d0385-5453-453d-9a0c-38dd5eed798e"",
                    ""path"": ""<NPad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2DVector"",
                    ""id"": ""ca3e4785-163a-476f-a3cb-9a13b863ca3b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3d34d093-61d6-4553-9f9c-8e4219bd1422"",
                    ""path"": ""<NPad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""RightAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9d616578-dda9-4380-ad06-7fbbac19495c"",
                    ""path"": ""<NPad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""RightAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""3076e047-2d8a-4904-bf1e-6f226786ba24"",
                    ""path"": ""<NPad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""RightAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4e306c64-1b20-48eb-b480-9a329505db6d"",
                    ""path"": ""<NPad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControllerScheme_NintendoSwitch"",
                    ""action"": ""RightAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Character_Destino"",
            ""id"": ""495213c2-f477-4bd0-b01d-45ecc05a0fdf"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""6de48d56-ad27-44a7-a4f0-795a1df184a8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftAxes"",
                    ""type"": ""Button"",
                    ""id"": ""3fe1fc98-e08a-40e6-9c0f-3a27aa188bde"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0328f304-0bcd-4d79-9823-3224e1dfdeef"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2DVector_DPad"",
                    ""id"": ""7073d5c8-7bf7-4c2d-ae10-cab3485cdb7d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""116b9e55-fe57-4e23-8176-033667e2ec42"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""589a466f-f0cc-497e-be62-bc68b9bb2d4c"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a5fad9b0-dcf1-4101-b489-1140f3bc9e89"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4e243006-bced-45bc-acda-2947e36d993c"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2DVector_WASD"",
                    ""id"": ""86b42f04-9632-451c-926c-0a6ab77ae2d6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4a9feb61-f7bb-480b-a44a-6ef11f2a878e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c2d5f736-394a-48a9-800b-eac7229be0ac"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fc70ba84-dec2-4f13-a7ae-e0b959d406be"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4fa992b5-d53b-4ff2-9ef3-78c8882ff6e4"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""ControlScheme_Keyboard&Mouse"",
                    ""action"": ""LeftAxes"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""ControllerScheme_NintendoSwitch"",
            ""bindingGroup"": ""ControllerScheme_NintendoSwitch"",
            ""devices"": [
                {
                    ""devicePath"": ""<NPad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""ControllerScheme_Xbox360"",
            ""bindingGroup"": ""ControllerScheme_Xbox360"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""ControlScheme_Keyboard&Mouse"",
            ""bindingGroup"": ""ControlScheme_Keyboard&Mouse"",
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
            // Player_Mateo
            m_Player_Mateo = asset.FindActionMap("Player_Mateo", throwIfNotFound: true);
            m_Player_Mateo_Jump = m_Player_Mateo.FindAction("Jump", throwIfNotFound: true);
            m_Player_Mateo_FireShooting_Frontal = m_Player_Mateo.FindAction("FireShooting_Frontal", throwIfNotFound: true);
            m_Player_Mateo_Attack_Sword = m_Player_Mateo.FindAction("Attack_Sword", throwIfNotFound: true);
            m_Player_Mateo_LeftAxes = m_Player_Mateo.FindAction("LeftAxes", throwIfNotFound: true);
            m_Player_Mateo_RightAxes = m_Player_Mateo.FindAction("RightAxes", throwIfNotFound: true);
            // Character_Destino
            m_Character_Destino = asset.FindActionMap("Character_Destino", throwIfNotFound: true);
            m_Character_Destino_Jump = m_Character_Destino.FindAction("Jump", throwIfNotFound: true);
            m_Character_Destino_LeftAxes = m_Character_Destino.FindAction("LeftAxes", throwIfNotFound: true);
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

        // Player_Mateo
        private readonly InputActionMap m_Player_Mateo;
        private IPlayer_MateoActions m_Player_MateoActionsCallbackInterface;
        private readonly InputAction m_Player_Mateo_Jump;
        private readonly InputAction m_Player_Mateo_FireShooting_Frontal;
        private readonly InputAction m_Player_Mateo_Attack_Sword;
        private readonly InputAction m_Player_Mateo_LeftAxes;
        private readonly InputAction m_Player_Mateo_RightAxes;
        public struct Player_MateoActions
        {
            private InputMaster m_Wrapper;
            public Player_MateoActions(InputMaster wrapper) { m_Wrapper = wrapper; }
            public InputAction @Jump => m_Wrapper.m_Player_Mateo_Jump;
            public InputAction @FireShooting_Frontal => m_Wrapper.m_Player_Mateo_FireShooting_Frontal;
            public InputAction @Attack_Sword => m_Wrapper.m_Player_Mateo_Attack_Sword;
            public InputAction @LeftAxes => m_Wrapper.m_Player_Mateo_LeftAxes;
            public InputAction @RightAxes => m_Wrapper.m_Player_Mateo_RightAxes;
            public InputActionMap Get() { return m_Wrapper.m_Player_Mateo; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(Player_MateoActions set) { return set.Get(); }
            public void SetCallbacks(IPlayer_MateoActions instance)
            {
                if (m_Wrapper.m_Player_MateoActionsCallbackInterface != null)
                {
                    Jump.started -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnJump;
                    Jump.performed -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnJump;
                    Jump.canceled -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnJump;
                    FireShooting_Frontal.started -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnFireShooting_Frontal;
                    FireShooting_Frontal.performed -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnFireShooting_Frontal;
                    FireShooting_Frontal.canceled -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnFireShooting_Frontal;
                    Attack_Sword.started -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnAttack_Sword;
                    Attack_Sword.performed -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnAttack_Sword;
                    Attack_Sword.canceled -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnAttack_Sword;
                    LeftAxes.started -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnLeftAxes;
                    LeftAxes.performed -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnLeftAxes;
                    LeftAxes.canceled -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnLeftAxes;
                    RightAxes.started -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnRightAxes;
                    RightAxes.performed -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnRightAxes;
                    RightAxes.canceled -= m_Wrapper.m_Player_MateoActionsCallbackInterface.OnRightAxes;
                }
                m_Wrapper.m_Player_MateoActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Jump.started += instance.OnJump;
                    Jump.performed += instance.OnJump;
                    Jump.canceled += instance.OnJump;
                    FireShooting_Frontal.started += instance.OnFireShooting_Frontal;
                    FireShooting_Frontal.performed += instance.OnFireShooting_Frontal;
                    FireShooting_Frontal.canceled += instance.OnFireShooting_Frontal;
                    Attack_Sword.started += instance.OnAttack_Sword;
                    Attack_Sword.performed += instance.OnAttack_Sword;
                    Attack_Sword.canceled += instance.OnAttack_Sword;
                    LeftAxes.started += instance.OnLeftAxes;
                    LeftAxes.performed += instance.OnLeftAxes;
                    LeftAxes.canceled += instance.OnLeftAxes;
                    RightAxes.started += instance.OnRightAxes;
                    RightAxes.performed += instance.OnRightAxes;
                    RightAxes.canceled += instance.OnRightAxes;
                }
            }
        }
        public Player_MateoActions @Player_Mateo => new Player_MateoActions(this);

        // Character_Destino
        private readonly InputActionMap m_Character_Destino;
        private ICharacter_DestinoActions m_Character_DestinoActionsCallbackInterface;
        private readonly InputAction m_Character_Destino_Jump;
        private readonly InputAction m_Character_Destino_LeftAxes;
        public struct Character_DestinoActions
        {
            private InputMaster m_Wrapper;
            public Character_DestinoActions(InputMaster wrapper) { m_Wrapper = wrapper; }
            public InputAction @Jump => m_Wrapper.m_Character_Destino_Jump;
            public InputAction @LeftAxes => m_Wrapper.m_Character_Destino_LeftAxes;
            public InputActionMap Get() { return m_Wrapper.m_Character_Destino; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(Character_DestinoActions set) { return set.Get(); }
            public void SetCallbacks(ICharacter_DestinoActions instance)
            {
                if (m_Wrapper.m_Character_DestinoActionsCallbackInterface != null)
                {
                    Jump.started -= m_Wrapper.m_Character_DestinoActionsCallbackInterface.OnJump;
                    Jump.performed -= m_Wrapper.m_Character_DestinoActionsCallbackInterface.OnJump;
                    Jump.canceled -= m_Wrapper.m_Character_DestinoActionsCallbackInterface.OnJump;
                    LeftAxes.started -= m_Wrapper.m_Character_DestinoActionsCallbackInterface.OnLeftAxes;
                    LeftAxes.performed -= m_Wrapper.m_Character_DestinoActionsCallbackInterface.OnLeftAxes;
                    LeftAxes.canceled -= m_Wrapper.m_Character_DestinoActionsCallbackInterface.OnLeftAxes;
                }
                m_Wrapper.m_Character_DestinoActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Jump.started += instance.OnJump;
                    Jump.performed += instance.OnJump;
                    Jump.canceled += instance.OnJump;
                    LeftAxes.started += instance.OnLeftAxes;
                    LeftAxes.performed += instance.OnLeftAxes;
                    LeftAxes.canceled += instance.OnLeftAxes;
                }
            }
        }
        public Character_DestinoActions @Character_Destino => new Character_DestinoActions(this);
        private int m_ControllerScheme_NintendoSwitchSchemeIndex = -1;
        public InputControlScheme ControllerScheme_NintendoSwitchScheme
        {
            get
            {
                if (m_ControllerScheme_NintendoSwitchSchemeIndex == -1) m_ControllerScheme_NintendoSwitchSchemeIndex = asset.FindControlSchemeIndex("ControllerScheme_NintendoSwitch");
                return asset.controlSchemes[m_ControllerScheme_NintendoSwitchSchemeIndex];
            }
        }
        private int m_ControllerScheme_Xbox360SchemeIndex = -1;
        public InputControlScheme ControllerScheme_Xbox360Scheme
        {
            get
            {
                if (m_ControllerScheme_Xbox360SchemeIndex == -1) m_ControllerScheme_Xbox360SchemeIndex = asset.FindControlSchemeIndex("ControllerScheme_Xbox360");
                return asset.controlSchemes[m_ControllerScheme_Xbox360SchemeIndex];
            }
        }
        private int m_ControlScheme_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme ControlScheme_KeyboardMouseScheme
        {
            get
            {
                if (m_ControlScheme_KeyboardMouseSchemeIndex == -1) m_ControlScheme_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("ControlScheme_Keyboard&Mouse");
                return asset.controlSchemes[m_ControlScheme_KeyboardMouseSchemeIndex];
            }
        }
        public interface IPlayer_MateoActions
        {
            void OnJump(InputAction.CallbackContext context);
            void OnFireShooting_Frontal(InputAction.CallbackContext context);
            void OnAttack_Sword(InputAction.CallbackContext context);
            void OnLeftAxes(InputAction.CallbackContext context);
            void OnRightAxes(InputAction.CallbackContext context);
        }
        public interface ICharacter_DestinoActions
        {
            void OnJump(InputAction.CallbackContext context);
            void OnLeftAxes(InputAction.CallbackContext context);
        }
    }
}
