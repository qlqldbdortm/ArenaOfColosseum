using System;
using Colosseum.UI.Lobby;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Colosseum.InputSystem.InputHandler
{
    public class UIInputHandler: InputHandlerBase
    {
        private Vector2 _moveDirection;
        private readonly float _moveSpeed = 0.1f;
        private float _lastMoveTime;
        private InputAction _navigateAction;
        private bool _isNavigateInputPressedLastFrame = false;
        protected override void Init()
        {
            InputActionMap actionMap = InputManager.GetActionMap(ActionMapType.UI);
            _navigateAction = actionMap.FindAction("Move");
            actionMap.AddAction("Move", OnMove, null, OnMove);
            actionMap.AddAction("Rotate", OnRotate);
            actionMap.AddAction("Select", OnSelect, null, OnSelect);
            actionMap.AddAction("Cancel", OnCancel, null, OnCancel);
            actionMap.AddAction("Menu", OnMenu,null, OnMenu);
            //InputManager.OnInputModeChanged += OnInputModeChanged;
        }

        private void OnEnable()
        {
            // 씬 로드 시 Action Map을 활성화하기 위해 이벤트에 등록합니다.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 오브젝트 비활성화 시 이벤트 등록을 해제합니다.
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init();
        }
        
        private void Update()
        {
            if (PopupManager.Instance != null && PopupManager.Instance.currentPopup != null)
            {
                HandlePopupInput();
                return;
            }
            
            // 이 로직은 OnMove와 충돌하지 않고 독립적으로 실행됩니다.
            if (_navigateAction != null)
            {
                // "Move" 액션의 Vector2 값을 직접 읽어옵니다.
                Vector2 moveInput = _navigateAction.ReadValue<Vector2>();

                // 스틱/방향키가 약간이라도 움직였는지 확인합니다.
                bool isNavigateInputPressed = moveInput.sqrMagnitude > 0.1f;

                // 입력이 없다가 -> 생긴 그 순간(Rising Edge)을 감지합니다.
                bool navigateStartedThisFrame = isNavigateInputPressed && !_isNavigateInputPressedLastFrame;

                // 다음 프레임을 위해 현재 입력 상태를 저장합니다.
                _isNavigateInputPressedLastFrame = isNavigateInputPressed;

                // 포커스가 없고, 패드 이동 입력이 '시작'되었을 때
                if (EventSystem.current.currentSelectedGameObject == null && navigateStartedThisFrame)
                {
                    // 현재 활성화된 FirstUISelecter를 찾아서 포커스를 복원합니다.
                    FirstUISelecter activeSelector = FindObjectOfType<FirstUISelecter>();
                    if (activeSelector != null)
                    {
                        activeSelector.SelectFirstUI();
                    }
                }
            }
        }

        #region 팝업 입력 처리

        private void HandlePopupInput()
        {
            var currentPopup = PopupManager.Instance.currentPopup;
            
            if (!currentPopup.canReceiveInput) return;

            var selectedObj = EventSystem.current.currentSelectedGameObject;
            
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (currentPopup.firstSelectableUI != null)
                {
                    EventSystem.current.SetSelectedGameObject(currentPopup.firstSelectableUI);
                }
            }
            
            if (selectedObj != null && !selectedObj.transform.IsChildOf(currentPopup.transform))
            {
                EventSystem.current.SetSelectedGameObject(currentPopup.firstSelectableUI);
            }
        }

        #endregion
        
        private void OnRotate(InputAction.CallbackContext context)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            if (context.canceled) dir = Vector2.zero;

            CharacterRotator.OnRotate?.Invoke(dir);
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            if (Time.unscaledTime < _lastMoveTime + _moveSpeed) return;

            _moveDirection = context.ReadValue<Vector2>();

            // EventSystem에 이동 이벤트를 직접 전달
            var moveEventData = new AxisEventData(EventSystem.current);

            // 수평 이동 감지
            if (Mathf.Abs(_moveDirection.x) > 0.1f)
            {
                moveEventData.moveDir = _moveDirection.x > 0 ? MoveDirection.Right : MoveDirection.Left;
            }
            // 수직 이동 감지
            else if (Mathf.Abs(_moveDirection.y) > 0.1f)
            {
                moveEventData.moveDir = _moveDirection.y > 0 ? MoveDirection.Up : MoveDirection.Down;
            }
            else
            {
                return; // 이동량이 충분하지 않으면 무시
            }

            // 현재 선택된 오브젝트에 이동 이벤트 실행
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, moveEventData, ExecuteEvents.moveHandler);
                _lastMoveTime = Time.unscaledTime;
            }
        }

        private void OnSelect(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            // 현재 선택된 UI 요소가 있는지 확인
            var currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected != null)
            {
                // 선택된 요소에서 ISubmitHandler 인터페이스를 찾아 실행
                var submitHandler = currentSelected.GetComponent<ISubmitHandler>();
                if (submitHandler != null)
                {
                    Debug.Log($"{currentSelected.name} submitted!");
                    submitHandler.OnSubmit(new BaseEventData(EventSystem.current));
                }
            }
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            //팝업이 있으면 팝업 닫기 우선
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.OnCancelInput();
            }
        }

        private void OnMenu(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            PlayerEventBus.OnToggleSettingsUI?.Invoke();
        }
    }
}