using System;
using System.Collections;
using System.Collections.Generic;
using Colosseum.Customizing;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Lobby
{
    public class CustomPanel : PopupBase
    {
        public Button customizeButton;
        public GameObject customUI;
        public Button exitButton;
        public void Awake()
        {
            customizeButton.onClick.AddListener(() => PopupManager.Instance.OpenPopup(this));
            exitButton.onClick.AddListener(OnExitButtonClicked);
            firstSelectableUI = exitButton.gameObject;
        }
        
        private async void OnExitButtonClicked()
        {
            try
            {
                // 1. 저장 먼저
                await DataManager.Instance.SaveToFirebase();
    
                // 2. 팝업 닫기
                ClosePopupAndResetFocus();
            }
            catch (Exception e)
            {
                DialogMessage.ShowMessage("오류발생",e.Message);
            }
            
        }
        
        #region PopupBase overide

        public override void OnPopupOpened()
        {
            base.OnPopupOpened();
            CustomManager.Instance.UIPanelRefresh();
            customUI.SetActive(true);
        }
        public override void OnPopupClosed()
        {
            base.OnPopupClosed();
            customUI.SetActive(false);
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
