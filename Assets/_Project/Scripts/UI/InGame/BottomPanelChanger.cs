using Colosseum.InputSystem;
using DG.Tweening;
using UnityEngine;

namespace Colosseum.UI.InGame
{
    /// <summary>
    /// Bottom UI가 InputMode에 맞춰 Panel을 교체하는 역할
    /// </summary>
    public class BottomPanelChanger: MonoBehaviour
    {
        [SerializeField] private CanvasGroup keyboardUIPanel;
        [SerializeField] private CanvasGroup gamepadUIPanel;
        
        
        void Awake()
        {
            InputManager.OnInputModeChanged += OnInputChanged;
        }
        void Start()
        {
            switch (InputManager.CurrentInputMode)
            {
                case InputMode.Gamepad:
                    keyboardUIPanel.transform.localPosition = Vector2.down * 50;
                    keyboardUIPanel.alpha = 0;
                    gamepadUIPanel.transform.localPosition = Vector2.zero;
                    keyboardUIPanel.alpha = 1;
                    break;
                case InputMode.KeyBoardAndMouse:
                default:
                    keyboardUIPanel.transform.localPosition = Vector2.zero;
                    keyboardUIPanel.alpha = 1;
                    gamepadUIPanel.transform.localPosition = Vector2.down * 50;
                    gamepadUIPanel.alpha = 0;
                    break;
            }
        }


        private void OnInputChanged()
        {
            print(InputManager.CurrentInputMode);
            switch (InputManager.CurrentInputMode)
            {
                case InputMode.Gamepad:
                    keyboardUIPanel.transform.DOLocalMoveY(-50, 0.5f);
                    keyboardUIPanel.DOFade(0, 0.5f);
                    gamepadUIPanel.transform.DOLocalMoveY(0, 0.5f);
                    gamepadUIPanel.DOFade(1, 0.5f);
                    break;
                case InputMode.KeyBoardAndMouse:
                default:
                    keyboardUIPanel.transform.DOLocalMoveY(0, 0.5f);
                    keyboardUIPanel.DOFade(1, 0.5f);
                    gamepadUIPanel.transform.DOLocalMoveY(-50, 0.5f);
                    gamepadUIPanel.DOFade(0, 0.5f);
                    break;
            }
        }
    }
}