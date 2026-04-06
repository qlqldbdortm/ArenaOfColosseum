using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Colosseum
{
    [System.Serializable] //PlayerPrefs에 저장할 예정
    public class GameSettingsData
    {
        public int resolutionWidth = 1920;
        public int resolutionHeight = 1080;
        public bool isFullScreen = true;

        public static GameSettingsData GetDefault()
        {
            return new GameSettingsData();
        }
    }
}