using System.Collections.Generic;
using Colosseum.Network.Lobby;
using Colosseum.UI.Audio;
using Colosseum.Unit;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.UI.Room
{
    public class RoomPanel: MonoBehaviour
    {
        [Tooltip("방 이름")] [SerializeField] private TextMeshProUGUI titleText;

        [Header("Player Panel Group")]
        [Tooltip("좌측 팀 그룹")] [SerializeField] private PlayerEntryPanel[] leftTeams;
        [Tooltip("우측 팀 그룹")] [SerializeField] private PlayerEntryPanel[] rightTeams;
        
        [Header("Buttons")]
        [Tooltip("준비 버튼")] [SerializeField] private Toggle readyToggle;
        [Tooltip("나가기 버튼")] [SerializeField] private Button exitButton;
        
        
        private PlayerEntryPanel[] playerPanels;
        private readonly Dictionary<Player, PlayerEntryPanel> indexDict = new();


        void Awake()
        {
            readyToggle.onValueChanged.AddListener(OnReadyToggleChanged);
            exitButton.onClick.AddListener(OnExitButtonClick);
        }

        void OnEnable()
        {
            readyToggle.isOn = false;
            titleText.text = PhotonNetwork.CurrentRoom.Name;
            SetPlayerSize(PhotonNetwork.CurrentRoom.MaxPlayers);
            
            RoomPunManager.OnPlayerSlotChanged += OnPlayerSlotChanged;
            RoomPunManager.OnMasterSwitched += OnMasterChanged;
            RoomPunManager.OnClassChanged += OnClassChanged;
            RoomPunManager.OnPlayerReadyChanged += OnPlayerReadyChanged;

            RoomPunManager.OnPlayerLeft += OnLeavePlayer;
        }
        void OnDisable()
        {
            foreach (var panel in playerPanels)
            {
                _ = panel.SetUserAsync(null);
            }
            indexDict.Clear();

            RoomPunManager.OnPlayerSlotChanged -= OnPlayerSlotChanged;
            RoomPunManager.OnMasterSwitched -= OnMasterChanged;
            RoomPunManager.OnClassChanged -= OnClassChanged;
            RoomPunManager.OnPlayerReadyChanged -= OnPlayerReadyChanged;
            
            RoomPunManager.OnPlayerLeft -= OnLeavePlayer;
        }


        #region ◇ UI Events ◇
        private void OnReadyToggleChanged(bool isOn)
        {
            if(isOn)
            {
                LobbyAudioManager.PlaySfx(SfxType.Ready);
            }

            PhotonNetwork.LocalPlayer.AddProperty((PropName.ROOM_READY, isOn));
            exitButton.interactable = !isOn;
            
            foreach (var panel in playerPanels)
            {
                panel.LockSlotChangeButton(!isOn);
            }
        }
        
        private void OnExitButtonClick()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region ◇ Photon Events ◇
        private void OnPlayerReadyChanged(Player player, bool isReady)
        {
            if (!indexDict.ContainsKey(player)) return;
            
            indexDict[player].OnReadyChanged(isReady);
        }
        private void OnClassChanged(Player player, CharacterClass newClass)
        {
            if (!indexDict.ContainsKey(player)) return;
            
            indexDict[player].OnClassChanged(newClass);

            if (player == PhotonNetwork.LocalPlayer)
            {
                if(newClass == CharacterClass.None)
                {
                    readyToggle.interactable = false;
                    readyToggle.isOn = false;
                }
                else
                {
                    readyToggle.interactable = true;
                }
            }
        }

        private void OnPlayerSlotChanged(Player player, int slot)
        {
            if (indexDict.ContainsKey(player))
            {
                if (indexDict[player].SlotNumber == slot) return;
                
                _ = indexDict[player].SetUserAsync(null);
            }

            _ = playerPanels[slot].SetUserAsync(player);
            indexDict[player] = playerPanels[slot];
        }

        private void OnMasterChanged(Player newMaster)
        {
            foreach (PlayerEntryPanel player in playerPanels)
            {
                player.OnMasterSelected(player.User == newMaster);
            }
        }

        private void OnLeavePlayer(Player leavePlayer)
        {
            if (indexDict.ContainsKey(leavePlayer))
            {
                _ = indexDict[leavePlayer].SetUserAsync(null);
            }
            indexDict.Remove(leavePlayer);
        }
        #endregion
        

        private void SetPlayerSize(int size)
        {
            foreach (PlayerEntryPanel entry in leftTeams)
            {
                entry.gameObject.SetActive(false);
            }
            foreach (PlayerEntryPanel entry in rightTeams)
            {
                entry.gameObject.SetActive(false);
            }

            playerPanels = new PlayerEntryPanel[size];
            for (int i = 0; i < size >> 1; i++)
            {
                PlayerEntryPanel lPanel = leftTeams[i];
                PlayerEntryPanel rPanel = rightTeams[i];
                lPanel.SlotNumber = i * 2;
                rPanel.SlotNumber = i * 2 + 1;
                
                (playerPanels[i * 2]        = lPanel).gameObject.SetActive(true);
                (playerPanels[i * 2 + 1]    = rPanel).gameObject.SetActive(true);
            }
        }
    }
}