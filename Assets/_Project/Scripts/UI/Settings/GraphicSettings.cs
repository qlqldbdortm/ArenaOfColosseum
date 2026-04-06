using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum
{
    public class GraphicSettings : MonoBehaviour
    {
        [Header("UI 컴포넌트")]
        public TMP_Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;
        
        private Resolution[] availableResolutions =
        {
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 1680, height = 1050 },
            new Resolution { width = 1366, height = 768 },
            new Resolution { width = 1280, height = 720 },
        };

        private void Start()
        {
            InitializeResolutionDropdown();
            
            resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
            
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
        }

        private void InitializeResolutionDropdown()
        {
            resolutionDropdown.ClearOptions();
            
            List<string> options = new List<string>();
            foreach (var resolution in availableResolutions)
            {
                string option = resolution.width + " x " + resolution.height;
                options.Add(option);
            }
            
            resolutionDropdown.AddOptions(options);
        }

        public void UpdateUI(GameSettingsData settings)
        {
            int resolutionIndex = FindResolutionIndex(settings.resolutionWidth, settings.resolutionHeight);
            if (resolutionIndex >= 0)
            {
                resolutionDropdown.value = resolutionIndex;
            }
            else
            {
                resolutionDropdown.value = 0;
            }
            
            //전체화면 토글 설정
            fullscreenToggle.isOn = settings.isFullScreen;  
        }

        private int FindResolutionIndex(int width, int height)
        {
            for (int i = 0; i < availableResolutions.Length; i++)
            {
                if (availableResolutions[i].width == width && availableResolutions[i].height == height)
                {
                    return i;
                }
            }
            return -1;
        }

        private void OnResolutionDropdownChanged(int index)
        {
            SettingsManager.Instance.tempSettings.resolutionWidth = availableResolutions[index].width;
            SettingsManager.Instance.tempSettings.resolutionHeight = availableResolutions[index].height;
            SettingsManager.Instance.ApplySettingsToGame(SettingsManager.Instance.tempSettings);
        }

        private void OnFullscreenToggleChanged(bool isFullscreen)
        {
            SettingsManager.Instance.tempSettings.isFullScreen = isFullscreen;
            SettingsManager.Instance.ApplySettingsToGame(SettingsManager.Instance.tempSettings);
        }
    }
}
