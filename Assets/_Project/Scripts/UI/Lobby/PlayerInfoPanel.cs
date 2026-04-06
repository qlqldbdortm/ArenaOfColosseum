using System;
using Colosseum.Authentication;
using Colosseum.Network;
using Colosseum.UI.Audio;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Lobby
{
    public class PlayerInfoPanel: MonoBehaviour
    {
        [Header("Player Info Panel")]
        [Tooltip("플레이어명 텍스트")] [SerializeField] private TextMeshProUGUI playerName;
        [Tooltip("플레이어 UID")] [SerializeField] private TextMeshProUGUI playerUid;
        [Tooltip("승수 카운터")] [SerializeField] private TextMeshProUGUI winText;
        [Tooltip("판수 카운터")] [SerializeField] private TextMeshProUGUI matchText;

        
        void OnEnable()
        {
            RefreshInfo();
        }

        
        private void RefreshInfo()
        {
            if (!GameDataManager.Instance.IsLoggedIn) return;
            
            UserData data = GameDataManager.Instance.PlayerData;
            string nickname = data.nickname;
            string uid = data.Uid;

            int winCount = data.wins;
            int matchCount = data.totalMatches;
            
            playerName?.SetText(nickname);
            playerUid?.SetText($"[{uid}]");
            
            winText?.SetText(matchCount > 0 ? $"승률: {winCount / (float)matchCount * 100:F1}%({winCount:N0})" : "-");
            matchText?.SetText($"판수: {matchCount:N0}");
        }
    }
}