using System;
using System.Collections;
using System.Collections.Generic;
using Colosseum.Core;
using UnityEngine;
using UnityEngine.UI;
using Colosseum.InputSystem.InputHandler;
using Colosseum.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//옵션 메뉴 관리 및 PlayerPrefs 저장
//각각의 옵션 메뉴는 각 Panel의 스크립트에서 관리

namespace Colosseum
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        private const string PREFS_PREFIX = "Colosseum_";
        private const string KEY_RESOLUTION_WIDTH = PREFS_PREFIX + "ResolutionWidth";
        private const string KEY_RESOLUTION_HEIGHT = PREFS_PREFIX + "ResolutionHeight";
        private const string KEY_FULLSCREEN = PREFS_PREFIX + "FullScreen";
        
        [Header("설정 데이터")]
        public GameSettingsData currentSettings; //현재 적용된 설정
        public GameSettingsData tempSettings; //임시 설정
        
        private GraphicSettings graphicSettings;
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            graphicSettings = GetComponent<GraphicSettings>();
        }
        
        private void Start()
        {
            currentSettings = LoadSettings();
            
            ApplySettingsToGame(currentSettings);
        }
        
        public void OnResetButtonClick()
        {
            tempSettings = GameSettingsData.GetDefault();
            graphicSettings.UpdateUI(tempSettings);
        }
        
        public GameSettingsData CopySettings(GameSettingsData settings)
        {
            GameSettingsData copy = new GameSettingsData();
            copy.resolutionWidth = settings.resolutionWidth;
            copy.resolutionHeight = settings.resolutionHeight;
            copy.isFullScreen = settings.isFullScreen;
            return copy;
        }
        
        #region PlayersPrefs 저장 기능
        /// <summary>
        /// 옵션 설정을 PlayerPrefs에 저장합니다.
        /// </summary>
        /// <param name="settings"></param>
        public void SaveSettings(GameSettingsData settings)
        {
            PlayerPrefs.SetInt(KEY_RESOLUTION_WIDTH, settings.resolutionWidth);
            PlayerPrefs.SetInt(KEY_RESOLUTION_HEIGHT, settings.resolutionHeight);
            PlayerPrefs.SetInt(KEY_FULLSCREEN, settings.isFullScreen ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// PlayerPrefs에서 설정을 불러옵니다.
        /// 저장된 설정이 없으면 기본값을 반환합니다.
        /// </summary>
        /// <returns></returns>
        private GameSettingsData LoadSettings()
        {
            GameSettingsData settings = new GameSettingsData();
            settings.resolutionWidth = PlayerPrefs.GetInt(KEY_RESOLUTION_WIDTH, 1920);
            settings.resolutionHeight = PlayerPrefs.GetInt(KEY_RESOLUTION_HEIGHT, 1080);
            settings.isFullScreen = PlayerPrefs.GetInt(KEY_FULLSCREEN, 1) == 1;
            return settings;
        }
        #endregion
        
        /// <summary>
        /// 설정을 실제 게임에 적용합니다. (해상도 변경 등)
        /// </summary>
        /// <param name="settings"></param>
        public void ApplySettingsToGame(GameSettingsData settings)
        {
            Screen.SetResolution(settings.resolutionWidth, settings.resolutionHeight, settings.isFullScreen);
        }
    }
}
