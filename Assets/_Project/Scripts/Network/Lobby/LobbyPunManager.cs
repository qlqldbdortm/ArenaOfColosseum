using System;
using System.Collections.Generic;
using Colosseum.Core;
using Colosseum.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Colosseum.Authentication;
using Colosseum.UI.Audio;
using UnityEngine;

namespace Colosseum.Network.Lobby
{
    public class LobbyPunManager: MonoBehaviourPunCallbacks
    {
        public static event Action<List<RoomInfo>> OnRoomListChanged = null;
        
        
        private readonly List<RoomInfo> roomListCache = new();


        public override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnDisable()
        {
            base.OnDisable();
        }
        
        
        public override void OnConnectedToMaster()
        {
            if (GameDataManager.Instance.IsLoggedIn && !PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }
        

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            roomListCache.AddRange(roomList);
            if (OnRoomListChanged == null) return;
            OnRoomListChanged?.Invoke(roomListCache);
            roomListCache.Clear();
        }

        public override void OnLeftLobby()
        {
            LobbyAudioManager.PlaySfx(SfxType.LobbyExit);
            MainPageManager.Instance.ChangePage(MainPageType.Auth);
        }
        public override void OnJoinedLobby()
        {
            MainPageManager.Instance.ChangePage(MainPageType.Lobby);
        }
        public override void OnLeftRoom()
        {
            LobbyAudioManager.PlaySfx(SfxType.RoomExit);
        }
        public override void OnJoinedRoom()
        {
            LobbyAudioManager.PlaySfx(SfxType.RoomEnter);
        }
    }
}