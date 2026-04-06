using Colosseum.Skill;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Colosseum.Authentication;
using Colosseum.InputSystem;
using Colosseum.Network.InGame.State;
using Colosseum.UI.InGame;
using Cysharp.Threading.Tasks;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Colosseum.Network.InGame
{
    public partial class InGameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("플레이어별 생성 위치")] public Transform[] spawnPoints;
        
        
        public static TeamType PlayerTeam { get; set; } = TeamType.Left;
        public bool TestMode { get; set; } = false;


        private readonly HashSet<int> statusBound = new ();
        private GameObject statusPrefab;

        private IGameState nowState = null;
        private readonly Dictionary<GameStateType, IGameState> states = new()
        {
            { GameStateType.Loading, new LoadingState()},
            { GameStateType.Waiting, new WaitingState()},
            { GameStateType.Battle, new BattleState()},
        };


        void Awake()
        {
            foreach (var state in states.Values)
            {
                state.OnInit(this);
            }
        }
        void Start()
        {
            ChangeState(GameStateType.Loading);
        }
        
        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.EventReceived += OnRaiseEventReceived;
        }
        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.EventReceived -= OnRaiseEventReceived;
        }


        public void ChangeState(GameStateType newType)
        {
            nowState?.OnExit();
            nowState = states.GetValueOrDefault(newType);
            nowState?.OnEnter();
        }
        
        
        void AttachStatus()
        {
            if (statusPrefab == null) statusPrefab = Resources.Load<GameObject>("Status");

            var allUnits = FindObjectsOfType<Unit.Unit>(includeInactive: false);
            foreach (var unit in allUnits)
            {
                var pv = unit.GetComponent<PhotonView>(); 
                
                if (pv == null) continue;
                if (pv.IsMine) continue;
                if (statusBound.Contains(pv.ViewID)) continue;
                var statusSlot = pv.transform.Find("StatusSlot");
                var uiObj = Instantiate(statusPrefab, statusSlot.position, Quaternion.identity);
                uiObj.transform.SetParent(statusSlot, false);
                uiObj.name = "Status (Remote)";
                
                statusBound.Add(pv.ViewID);
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            StartCoroutine(AttachNextFrame());

            IEnumerator AttachNextFrame()
            {
                yield return null;
                AttachStatus();
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Main");
        }


        private void OnRaiseEventReceived(EventData data)
        {
            RaiseEventType eventType = (RaiseEventType)data.Code;
            if (eventType == RaiseEventType.HitObjectHits)
            {
                string str = (string)data.CustomData;
                if (HitObject.SpawnedObjects.TryGetValue(str, out HitObject hitObject))
                {
                    // TODO: hitobject impaleCount 추가
                }
            }
            else if (eventType == RaiseEventType.HitObjectRelease)
            {
                string str = (string)data.CustomData;
                if (HitObject.SpawnedObjects.TryGetValue(str, out HitObject hitObject))
                {
                    hitObject.Release();
                }
            }
            else if (eventType == RaiseEventType.UnitDie)
            {
                Player sender = PhotonNetwork.CurrentRoom.GetPlayer(data.Sender);
                CheckPlayerDie(sender);
            }
            else if (eventType == RaiseEventType.RoundStart)
            {
                RoundPanel.ShowRoundPanel();
                InputManager.ChangeActionMap(ActionMapType.Player);
            }
            else if (eventType == RaiseEventType.RoundEnd)
            {
                TeamType victoryTeam = (TeamType)data.CustomData;
                GameDataManager.Instance.PlayerData.totalMatches++;
                if (PlayerTeam == victoryTeam)
                {
                    RoundPanel.ShowVictoryPanel();
                    
                    GameDataManager.Instance.PlayerData.wins++;
                }
                else
                {
                    RoundPanel.ShowDefeatPanel();
                }
                _ = GameDataManager.Instance.PlayerData.SaveAsync();
                _ = ChangeSceneAsync();
            }
        }


        [ContextMenu("To Main")]
        private async UniTask ChangeSceneAsync()
        {
            await UniTask.WaitForSeconds(3f);
            
            PhotonNetwork.LeaveRoom();
        }
    }
}