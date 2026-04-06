using Colosseum.Core;
using System;
using Colosseum.UI.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Colosseum.UI
{
    public class DialogMessage: PopupBase
    {
        private static DialogMessage instance;
        public static DialogMessage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DialogMessage>();
                }
                return instance;
            }
        }
        
        public static event Action OnDialogOpened;
        public static event Action OnDialogClosed;
        
        
        [Tooltip("메시지 팝업")] [SerializeField] private GameObject popupPanel;
        [Tooltip("메시지 제목")] [SerializeField] private TextMeshProUGUI titleText;
        [Tooltip("메시지 본문")] [SerializeField] private TextMeshProUGUI contentText;
        [Tooltip("확인 버튼")] [SerializeField] private Button confirmButton;


        protected void Awake()
        {
            instance = this;
            
            // PopupBase 설정
            canBeCanceled = true;
            canReceiveInput = true;
            
            // 확인 버튼이 처음 선택될 UI
            if (confirmButton != null)
            {
                firstSelectableUI = confirmButton.gameObject;
                //confirmButton.onClick.AddListener(CloseMessage);
            }
            popupPanel.SetActive(false);
        }


        public static void ShowMessage(string title, string message)
        {
            if (Instance is null)
            {
                Debug.Log($"{title}: {message}");
                return; // DialogMessage가 존재하지 않으면 호출이 불가능함.
            }

            LobbyAudioManager.PlaySfx(SfxType.PopupOpen);
            Instance.popupPanel.SetActive(true);
            Instance.titleText.text = title;
            Instance.contentText.text = message;
            
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.OpenPopup(Instance);
            }
            
            OnDialogOpened?.Invoke();
        }
        public static void ShowMessage(string message) => ShowMessage("알림", message);
        
        public static void CloseMessage()
        {
            if (Instance is null) return;
            if (PopupManager.Instance != null)
            {
                LobbyAudioManager.PlaySfx(SfxType.PopupClose);
                PopupManager.Instance.ClosePopup(Instance);
            }

            OnDialogClosed?.Invoke();
        }
        
        #region PopupBase 오버라이드

        public override void OnPopupOpened()
        {
            base.OnPopupOpened();
            popupPanel.SetActive(true);
        }

        public override void OnPopupClosed()
        {
            base.OnPopupClosed();
            popupPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }

        public override void OnPopupDeactivated()
        {
            base.OnPopupDeactivated();
            canReceiveInput = false;
        }

        public override void OnPopupReActivated()
        {
            base.OnPopupReActivated();
            canReceiveInput = true;
        }

        #endregion
    }
}