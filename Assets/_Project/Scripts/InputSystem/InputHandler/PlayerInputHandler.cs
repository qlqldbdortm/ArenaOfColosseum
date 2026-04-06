using UnityEngine;
using UnityEngine.InputSystem;

namespace Colosseum.InputSystem.InputHandler
{
    public class PlayerInputHandler: InputHandlerBase
    {
        [SerializeField] private LayerMask groundLayer;


        private Camera mainCamera;
        private bool clicked = false;
        
        
        protected override void Init()
        {
            mainCamera = Camera.main;
            
            InputActionMap actionMap = InputManager.GetActionMap(ActionMapType.Player);
            actionMap.AddAction("Move", OnMove);
            actionMap.AddAction("Look", OnLook);
            actionMap.AddAction("Emote", OnEmote);

            actionMap.AddAction("Sprint", OnSprint, null, OnSprint);
            actionMap.AddAction("NormalAttack", OnNormalAttack);
            actionMap.AddAction("HardAttack", OnHardAttack);
            actionMap.AddAction("Skill1", OnSkill1);
            actionMap.AddAction("Skill2", OnSkill2);
            actionMap.AddAction("Menu", OnMenu);
        }


        void Update()
        {
            if (InputManager.CurrentInputMode != InputMode.KeyBoardAndMouse) return;
            if (InputManager.CurrentActionMap != ActionMapType.Player) return;
            if (!clicked) return;
            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                PlayerEventBus.OnLook?.Invoke(hit.point);
            }
        }


        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            if (context.canceled) dir = Vector2.zero;
            
            PlayerEventBus.OnMove?.Invoke(dir);
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            if (context.canceled || dir.magnitude < 0.25f) return;
            
            PlayerEventBus.OnLookDir?.Invoke(dir);
        }
        
        private void OnEmote(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();

            if (!Mathf.Approximately(dir.x, 0))
            {
                if (dir.x > 0.5f)
                {
                    PlayerEventBus.OnEmote?.Invoke(1);
                }
                else
                {
                    PlayerEventBus.OnEmote?.Invoke(3);
                }
            }
            if (!Mathf.Approximately(dir.y, 0))
            {
                if (dir.y > 0.5f)
                {
                    PlayerEventBus.OnEmote?.Invoke(0);
                }
                else
                {
                    PlayerEventBus.OnEmote?.Invoke(2);
                }
            }
        }
        
        private void OnSprint(InputAction.CallbackContext context)
        {
            PlayerEventBus.OnSprint(context.started);
        }


        private void OnNormalAttack(InputAction.CallbackContext context)
        {
            if (!context.canceled && InputManager.CurrentInputMode == InputMode.KeyBoardAndMouse)
            {
                clicked = true;
            }
            else if(context.canceled)
            {
                clicked = false;
                PlayerEventBus.OnNormalAttack?.Invoke();
            }
        }

        private void OnHardAttack(InputAction.CallbackContext context)
        {
            if (!context.canceled && InputManager.CurrentInputMode == InputMode.KeyBoardAndMouse)
            {
                clicked = true;
            }
            else if(context.canceled)
            {
                clicked = false;
                PlayerEventBus.OnHardAttack?.Invoke();
            }
        }

        private void OnSkill1(InputAction.CallbackContext context)
        {
            if (!context.canceled && InputManager.CurrentInputMode == InputMode.KeyBoardAndMouse)
            {
                clicked = true;
            }
            else if(context.canceled)
            {
                clicked = false;
                PlayerEventBus.OnSkill1?.Invoke();
            }
        }
        
        private void OnSkill2(InputAction.CallbackContext context)
        {
            if (!context.canceled && InputManager.CurrentInputMode == InputMode.KeyBoardAndMouse)
            {
                clicked = true;
            }
            else if(context.canceled)
            {
                clicked = false;
                PlayerEventBus.OnSkill2?.Invoke();
            }
        }
        
        private void OnMenu(InputAction.CallbackContext context)
        {
            /*Debug.Log("메뉴 키 눌림");
            
            if (InputManager.CurrentActionMap == ActionMapType.Player)
            {
                PlayerEventBus.OnToggleSettingsUI.Invoke();
                InputManager.ChangeActionMap(ActionMapType.UI);
            }*/
        }
    }
}