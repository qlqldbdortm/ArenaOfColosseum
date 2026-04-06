using System.Collections.Generic;
using Colosseum.Authentication;
using Colosseum.Network.Lobby;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Colosseum.UI.Lobby
{
    public class RoomListPanel: MonoBehaviour
    {
        [Header("Room List")]
        [SerializeField] private RoomItem roomItemPrefab;
        [SerializeField] private Transform roomGroup;
        
        [Header("Bottom Buttons")]
        [SerializeField] private Button quickButton;
        [SerializeField] private Button createPanelOpenButton;
        
        [Header("Sound Effects")]
        [SerializeField] private AudioSource createPanelSound;
        [SerializeField] private AudioSource closeCreatePanelSound;
        
        [Header("룸 생성 PopupBase")]
        public PopupBase createPanelPopupBase;
        
        private readonly Dictionary<string, RoomItem> rooms = new();


        void Awake()
        {
            quickButton.onClick.AddListener(OnQuickButtonClick);
            
            createPanelOpenButton.onClick.AddListener(OnCreatePanelOpenButtonClick);
        }
        
        void OnEnable()
        {
            LobbyPunManager.OnRoomListChanged += OnRoomListChanged;
        }
        void OnDisable()
        {
            LobbyPunManager.OnRoomListChanged -= OnRoomListChanged;
        }


        private void OnQuickButtonClick()
        {
            RoomOptions option = new()
            {
                MaxPlayers = Constants.MAX_PLAYERS
            };
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"{GameDataManager.Instance.PlayerData.Uid}의 게임", roomOptions: option);
        }

        private void OnCreatePanelOpenButtonClick()
        {
            createPanelSound.Play();
            PopupManager.Instance.OpenPopup(createPanelPopupBase);
        }

        private void OnCloseCreatePanelButtonClick()
        {
            closeCreatePanelSound.Play();
        }
        
        private async UniTask CreateRoom(string roomName, RoomOptions option)
        {
            await UniTask.WaitUntil(() => PhotonNetwork.InLobby);
            PhotonNetwork.CreateRoom(roomName, option);
        }

        #region ◇ OnRoomListChanged: RoomList 갱신 ◇
        private void OnRoomListChanged(List<RoomInfo> roomList)
        {
            foreach (var room in roomList)
            {
                if (room.RemovedFromList)
                {
                    RemoveRoom(room);
                }
                else if (rooms.ContainsKey(room.Name))
                {
                    RefreshRoom(room);
                }
                else
                {
                    AddRoom(room);
                }
            }
        }

        private void RefreshRoom(RoomInfo roomInfo)
        {
            RoomItem roomItem = rooms[roomInfo.Name];
            roomItem.SetPlayerCount(roomInfo.PlayerCount);
        }
        private void AddRoom(RoomInfo roomInfo)
        {
            string roomName = roomInfo.Name;
            RoomItem roomItem = Instantiate(roomItemPrefab, roomGroup);
            roomItem.Init(roomName, roomInfo.MaxPlayers);
            roomItem.SetPlayerCount(roomInfo.PlayerCount);
            
            rooms.Add(roomName, roomItem);
        }
        private void RemoveRoom(RoomInfo roomInfo)
        {
            string roomName = roomInfo.Name;
            if (!rooms.TryGetValue(roomName, out RoomItem roomItem)) return;
            
            Destroy(roomItem.gameObject);
            rooms.Remove(roomName);
        }
        #endregion
    }
}