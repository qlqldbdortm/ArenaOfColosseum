using System;
using System.Collections;
using System.Collections.Generic;
using Colosseum.InputSystem;
using Colosseum.InputSystem.InputHandler;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Colosseum
{
    public class SettingsUI : PopupBase
    {
        [Header("세팅 패널")]
        public GameObject settingsPanel;
        
        [Header("세팅 버튼")]
        public Button graphicButton;
        
        [Header("그래픽 설정")]
        public GameObject graphicPanel;
        
        [Header("기능 버튼")]
        public Button closeButton;
        public Button resetButton;
        public Button confirmButton;
        
        private void Awake()
        {
            firstSelectableUI = graphicButton.gameObject;
            canBeCanceled = true;
            
            settingsPanel.SetActive(false);
            graphicButton.onClick.AddListener(OnGraphicButtonClick);
            closeButton.onClick.AddListener(CloseSettings);
            confirmButton.onClick.AddListener(ConfirmAndCloseSettings);

            if (SettingsManager.Instance != null)
            {
                resetButton.onClick.AddListener(SettingsManager.Instance.OnResetButtonClick);
            }
        }

        private void OnEnable()
        {
            PlayerEventBus.OnToggleSettingsUI += OnToggleRequested;
        }

        private void OnDisable()
        {
            PlayerEventBus.OnToggleSettingsUI -= OnToggleRequested;
        }

        private void OnToggleRequested()
        {
            if (settingsPanel.activeSelf)
            {
                PopupManager.Instance.CloseCurrentPopup();
            }
            else
            {
                OpenSettings();
            }
        }
        
        private void OpenSettings()
        {
            SettingsManager.Instance.tempSettings = 
                SettingsManager.Instance.CopySettings(SettingsManager.Instance.currentSettings);
            GetComponent<GraphicSettings>().UpdateUI(SettingsManager.Instance.tempSettings);
            
            PopupManager.Instance.OpenPopup(this);
        }

        private void CloseSettings()
        {
            SettingsManager.Instance.ApplySettingsToGame(SettingsManager.Instance.currentSettings);
            GetComponent<GraphicSettings>().UpdateUI(SettingsManager.Instance.currentSettings);
            ClosePopupAndResetFocus();
        }

        private void ConfirmAndCloseSettings()
        {
            SettingsManager.Instance.currentSettings =
                SettingsManager.Instance.CopySettings(SettingsManager.Instance.tempSettings);
            SettingsManager.Instance.ApplySettingsToGame(SettingsManager.Instance.currentSettings);
            SettingsManager.Instance.SaveSettings(SettingsManager.Instance.currentSettings);
            
            ClosePopupAndResetFocus();
        }

        #region PopupBase overide

        public override void OnPopupOpened()
        {
            base.OnPopupOpened();
            settingsPanel.SetActive(true);
        }
        public override void OnPopupClosed()
        {
            base.OnPopupClosed();
            settingsPanel.SetActive(false);
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
        
        private void OnGraphicButtonClick()
        {
            graphicPanel.SetActive(!graphicPanel.activeSelf);
        }
    }
}
