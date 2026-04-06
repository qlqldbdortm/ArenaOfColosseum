using System;
using System.Collections;
using System.Collections.Generic;
using Colosseum.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Colosseum
{
    public class PopupManager : Singleton<PopupManager> //캔버스의 컴포넌트로 사용
    {
        [Header("설정")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private CanvasGroup mainGanvasGroup;

        //팝업 스택 (나중에 연 팝업이 위에)
        private Stack<PopupBase> popupStack = new Stack<PopupBase>();
        
        //현재 활성화된 팝업
        public PopupBase currentPopup => popupStack.Count > 0 ? popupStack.Peek() : null;

        protected override void Awake()
        {
            base.Awake();

            if (mainCanvas == null)
            {
                mainCanvas = GetComponent<Canvas>();
            }
            if (mainGanvasGroup == null)
            {
                mainGanvasGroup = GetComponent<CanvasGroup>();
            }
        }

        private void Update()
        {
            if (popupStack.Count >= 1)
            {
                mainGanvasGroup.interactable = false;
                mainGanvasGroup.blocksRaycasts = false;
            }
            else
            {
                mainGanvasGroup.interactable = true;
                mainGanvasGroup.blocksRaycasts = true;           
            }
        }

        #region Popup 열기/닫기

        public void OpenPopup(PopupBase popup)
        {
            if (popup == null)
            {
                Debug.LogError("[PopupManager] 팝업이 없습니다! ]");
                return;
            }
            
            //이전에 켜진 팝업 비활성화
            if (currentPopup != null)
            {
                currentPopup.OnPopupDeactivated();
            }
            
            popupStack.Push(popup);
            popup.OnPopupOpened();

            SelectFirstUI(popup);
        }

        public void CloseCurrentPopup()
        {
            if (popupStack.Count == 0)
            {
                //Debug.LogWarning("[PopupManager] : 닫을 팝업창이 없습니다!");
                return;
            }
            
            PopupBase popup = popupStack.Pop();
            popup.OnPopupClosed();

            if (currentPopup != null)
            {
                currentPopup.OnPopupReActivated();
                SelectFirstUI(currentPopup);
            }
        }

        public void ClosePopup(PopupBase popup)
        {
            if (currentPopup == popup)
            {
                CloseCurrentPopup();
            }
            else
            {
                Debug.LogWarning($"[PopupManager] : {popup.name}은 현재 팝업이 아닙니다!");
            }
        }
        
        public void CloseAllPopup()
        {
            while (popupStack.Count > 0)
            {
                CloseCurrentPopup();
            }
        }
        
        #endregion

        #region 입력 제어

        private void SelectFirstUI(PopupBase popup)
        {
            if (popup.firstSelectableUI != null)
            {
                EventSystem.current.SetSelectedGameObject(popup.firstSelectableUI);
            }
        }
        
        public bool CanCurrentPopupReceiveInput()
        {
            return currentPopup != null && currentPopup.canReceiveInput;
        }

        public void OnCancelInput()
        {
            if (currentPopup != null && currentPopup.canBeCanceled)
            {
                CloseCurrentPopup();
            }
        }

        #endregion
    }
}
