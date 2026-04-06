using System;
using Colosseum.InputSystem.InputHandler;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Colosseum.InputSystem
{
    /// <summary>
    /// Input 정보를 관리하기 위한 Manager
    /// </summary>
    public class InputManager: MonoBehaviour
    {
        /// <summary>
        /// InputMode가 변경됐을 때 동작하는 Event
        /// </summary>
        public static Action OnInputModeChanged { get; set; } = null;
        /// <summary>
        /// Scene이 변경될 때 동작하는 Event<br/>
        /// 정확하게는 SceneUnloaded Event
        /// </summary>
        public static Action OnSceneChanged { get; set; } = null;
        /// <summary>
        /// 현재 사용중인 ActionMep 종류
        /// </summary>
        public static ActionMapType CurrentActionMap { get; private set; } = ActionMapType.UI;
        /// <summary>
        /// 현재 사용중인 Input 종류
        /// </summary>
        public static InputMode CurrentInputMode { get; private set; } = InputMode.None;
        
        private static PlayerInput PlayerInput { get; set; } = null;


        void Awake()
        {
            if (PlayerInput is null)
            {
                DontDestroyOnLoad(gameObject);
                
                PlayerInput = GetComponent<PlayerInput>();
                PlayerInput.onControlsChanged += OnInputChanged;
                
                SceneManager.sceneLoaded += OnSceneLoad;
                SceneManager.sceneUnloaded += OnSceneUnload;
                
                // --- 강제 초기화 코드 추가 ---
                if (PlayerInput.currentControlScheme != null)
                {
                    Debug.Log($"강제 초기화 시도: {PlayerInput.currentControlScheme}");
                    OnInputChanged(PlayerInput);
                }
                // --- 강제 초기화 코드 끝 ---

                SceneManager.LoadScene("Main");
            }
        }

        /// <summary>
        /// 활성화된 ActionMap을 변경
        /// </summary>
        /// <param name="actionMapType"></param>
        public static void ChangeActionMap(ActionMapType actionMapType)
        {
            CurrentActionMap = actionMapType;
            PlayerInput.SwitchCurrentActionMap(actionMapType.ToString());
        }
        /// <summary>
        /// ActionMap 객체를 반환
        /// </summary>
        /// <param name="actionMapType"></param>
        /// <returns></returns>
        public static InputActionMap GetActionMap(ActionMapType actionMapType) => PlayerInput.actions.FindActionMap(actionMapType.ToString(), true);


        private static void OnInputChanged(PlayerInput playerInput)
        {
            Debug.Log($"Input Changed: {playerInput.currentControlScheme}");
            switch (playerInput.currentControlScheme)
            {
                case "Gamepad":
                    CurrentInputMode = InputMode.Gamepad;
                    Cursor.visible = false;
                    break;
                case "KeyboardMouse":
                default:
                    CurrentInputMode = InputMode.KeyBoardAndMouse;
                    Cursor.visible = true;
                    break;
            }
            OnInputModeChanged?.Invoke();
        }


        private static void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "InGame")
            {
                ChangeActionMap(ActionMapType.Lock); // 인게임 씬 진입 시 일단 Lock으로 전환
            }
            else // 로그인, 로비 등 그 외 모든 씬
            {
                ChangeActionMap(ActionMapType.UI); // UI 위주 씬 진입 시 UI 맵으로 전환
            }
        }
        private static void OnSceneUnload(Scene scene)
        {
            PlayerEventBus.Clear();
            OnSceneChanged?.Invoke();
            OnSceneChanged = null;
            OnInputModeChanged = null;
        }
    }
}