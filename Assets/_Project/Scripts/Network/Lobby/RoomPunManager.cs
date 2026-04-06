using System;
using System.Text;
using System.Threading;
using Colosseum.Core;
using Colosseum.InputSystem;
using Colosseum.UI;
using Colosseum.UI.Room;
using Colosseum.Unit;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Colosseum.Network.Lobby
{
    public class RoomPunManager: PhotonSingleton<RoomPunManager>
    {
        public static event Action<Player> OnPlayerLeft = null;
        public static event Action<Player, int> OnPlayerSlotChanged = null;
        public static event Action<Player, bool> OnPlayerReadyChanged = null;
        public static event Action<Player> OnMasterSwitched = null; // TODO: 방장한테 별달아주는 기능용
        public static event Action<Player, CharacterClass> OnClassChanged = null; // TODO: 방장한테 별달아주는 기능용


        private Player[] roomPlayers = null;
        private CancellationTokenSource startToken = null;


        public override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnDisable()
        {
            base.OnDisable();
        }


        public override void OnLeftRoom()
        {
            Debug.LogWarning("방 나갔다이");
            ReadyTimerPanel.StopTimer();
        }

        public override void OnJoinedRoom()
        {
            Debug.LogWarning("룸 들어갔다이");
            MainPageManager.Instance.ChangePage(MainPageType.Room);
            PhotonNetwork.LocalPlayer.AddProperty((PropName.CLASS_TYPE, CharacterClass.None), (PropName.ROOM_READY, false));
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (!player.CustomProperties.ContainsKey(PropName.SLOT_NUMBER)) continue;
                OnPlayerSlotChanged?.Invoke(player, (int) player.CustomProperties[PropName.SLOT_NUMBER]);
            }
            
            if (!PhotonNetwork.IsMasterClient) return;
            
            roomPlayers = new Player[PhotonNetwork.CurrentRoom.MaxPlayers];
            for (int i = 0; i < roomPlayers.Length; i++) roomPlayers[i] = null;
            OnPlayerEnteredRoom(PhotonNetwork.LocalPlayer);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            for (int i = 0; i < roomPlayers.Length; i++)
            {
                if (roomPlayers[i] != null) continue;
                
                roomPlayers[i] = newPlayer;
                OnGiftSlot(newPlayer, i);
                return;
            }
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            OnPlayerLeft?.Invoke(otherPlayer);

            if (!PhotonNetwork.IsMasterClient) return;
            
            for (int i = 0; i < roomPlayers.Length; i++)
            {
                if (roomPlayers[i] != otherPlayer) continue;

                roomPlayers[i] = null;
                break;
            }

            CheckReadyByMaster();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            OnMasterSwitched?.Invoke(newMasterClient);
            
            if (PhotonNetwork.LocalPlayer != newMasterClient) return;

            roomPlayers = new Player[PhotonNetwork.CurrentRoom.MaxPlayers];
            for (int i = 0; i < roomPlayers.Length; i++) roomPlayers[i] = null;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                int idx = (int)player.CustomProperties[PropName.SLOT_NUMBER];
                roomPlayers[idx] = player;
            }

            ShowMessage(newMasterClient, "방장 변경", "새로운 방장이 되었습니다.");
            CheckReadyByMaster();
        }


        public override void OnRoomPropertiesUpdate(Hashtable changed)
        {
            StringBuilder sb = new();
            foreach (var c in changed.Keys)
            {
                sb.Append($"{c} ");
            }
            Debug.Log(sb.ToString());
            //changed.ContainsKey()
        }
        
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(PropName.SLOT_NUMBER)) // 슬롯 변경
            {
                OnPlayerSlotChanged?.Invoke(targetPlayer, (int) changedProps[PropName.SLOT_NUMBER]);
            }
            if (changedProps.ContainsKey(PropName.CLASS_TYPE))
            {
                OnClassChanged?.Invoke(targetPlayer, (CharacterClass) changedProps[PropName.CLASS_TYPE]);
            }
            if (changedProps.ContainsKey(PropName.ROOM_READY))
            {
                OnPlayerReadyChanged?.Invoke(targetPlayer, (bool) changedProps[PropName.ROOM_READY]);

                if (PhotonNetwork.IsMasterClient)
                {
                    CheckReadyByMaster();
                }
            }
        }


        #region ◇ Ready 관련 기능 ◇
        /// <summary>
        /// Master가 레디를 체크하는 용도
        /// </summary>
        private void CheckReadyByMaster()
        {
            #if UNITY_EDITOR // Unity Editor에서는 1인 테스트를 방장 스타트로 바꿈
            if (PhotonNetwork.LocalPlayer.CustomProperties.GetValueOrDefault(PropName.ROOM_READY, false))
            {
                ConfirmStart();
                return;
            }
            #endif
            bool hasAllReady = true;
            bool hasAllClass = true;
            bool hasLeft = false, hasRight = false;
            
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                // 준비 안 한 플레이어 있는지 체크
                if (!player.CustomProperties.GetValueOrDefault(PropName.ROOM_READY, false))
                {
                    hasAllReady = false;
                }
                if (player.CustomProperties.GetValueOrDefault(PropName.CLASS_TYPE, CharacterClass.None) == CharacterClass.None)
                {
                    hasAllClass = false;
                }

                // 각 팀에 최소 1명 이상이 있는지 체크
                int teamNumber = player.CustomProperties.GetValueOrDefault(PropName.TEAM_NUMBER, -1);
                if (teamNumber == -1)
                {
                    CancelStart();
                    return; // 아직 자리배치를 못받았다는 의미이므로, 준비를 취소해야 함
                }
                
                if (teamNumber == 0) hasLeft = true;
                if (teamNumber == 1) hasRight = true;
            }

            if (hasAllReady && hasAllClass && hasLeft && hasRight) // 준비 완료 상태라면
            {
                ConfirmStart();
            }
            else // 아니라면
            {
                CancelStart();
            }
        }
        /// <summary>
        /// 시작을 위한 준비 완료
        /// </summary>
        private void ConfirmStart()
        {
            if (startToken != null) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            photonView.RPC("OnReadyStartRpc", RpcTarget.All, Constants.START_TIMER);
            _ = StartWaitAsync(Constants.START_TIMER);
        }
        /// <summary>
        /// 시작을 위한 준비 완료 아님
        /// </summary>
        private void CancelStart()
        {
            if (startToken == null) return;
   
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            photonView.RPC("OnReadyStopRpc", RpcTarget.All);
            startToken?.Cancel();
            startToken = null;
        }

        private async UniTask StartWaitAsync(float seconds)
        {
            startToken = new();
            await UniTask.WaitForSeconds(seconds, cancellationToken: startToken.Token);
            PhotonNetwork.LoadLevel("InGame");
            startToken = null;
        }

        [PunRPC]
        private void OnReadyStartRpc(float seconds, PhotonMessageInfo info)
        {
            float netDelay = (float)(PhotonNetwork.Time - info.SentServerTime);
            
            ReadyTimerPanel.StartTimer(seconds - netDelay);
        }
        [PunRPC]
        private void OnReadyStopRpc()
        {
            ReadyTimerPanel.StopTimer();
        }
        #endregion


        #region ◇ 자리 변경 기능 ◇
        public void TrySlotChange(int slot)
        {
            photonView.RPC("OnRequestSlotChangeRpc", RpcTarget.MasterClient, slot);
        }
        [PunRPC]
        private void OnRequestSlotChangeRpc(int slot, PhotonMessageInfo info)
        {
            if (roomPlayers[slot] == null)
            {
                OnGiftSlot(info.Sender, slot);
            }
            else
            {
                ShowMessage(info.Sender, "자리이동 실패", "자리이동에 실패하셨습니다.");
            }
        }
        
        private void OnGiftSlot(Player player, int slot)
        {
            for (int i = 0; i < roomPlayers.Length; i++)
            {
                if (roomPlayers[i] != player) continue;
                    
                roomPlayers[i] = null;
                break;
            }
            roomPlayers[slot] = player;
            photonView.RPC("OnGiftSlotRpc", player, slot);
        }
        [PunRPC]
        private void OnGiftSlotRpc(int slotNumber)
        {
            PhotonNetwork.LocalPlayer.AddProperty(
                (PropName.SLOT_NUMBER, slotNumber), // 슬롯 번호 확정짓는 부분
                (PropName.TEAM_NUMBER, slotNumber % 2)); // TODO: 나중에는 팀 정하는 방식이 달라질 수도 있음
        }
        #endregion


        private void ShowMessage(Player player, string title, string message)
        {
            photonView.RPC("OnTakeMessageRpc", player, title, message);
        }
        [PunRPC]
        private void OnTakeMessageRpc(string title, string message)
        {
            DialogMessage.ShowMessage(title, message);
        }
    }
}