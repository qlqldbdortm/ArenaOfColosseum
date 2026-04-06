using System;
using System.Threading;
using Colosseum.Authentication;
using Colosseum.Customizing;
using Colosseum.InputSystem;
using Colosseum.InputSystem.InputHandler;
using Colosseum.LifeCycle;
using Colosseum.Unit;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Colosseum.Network.InGame.State
{
    public class LoadingState: IGameState
    {
        private InGameManager manager = null;
        private CancellationTokenSource token = null;
        
        
        public void OnInit(InGameManager manager)
        {
            this.manager = manager;
        }
        
        public void OnEnter()
        {
            _ = LoadingAsync();
        }

        public void OnExit()
        {
            token?.Cancel();
            token = null;
        }


        private async UniTask LoadingAsync()
        {
            token = new();
            
            
            if (!PhotonNetwork.IsConnected)
            {
                manager.TestMode = true;

                Debug.Log("접속 시도");
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.CreateRoom("TestRoom");
            }
            await UniTask.WaitUntil(() => PhotonNetwork.InRoom);

            
            var slotNum = PhotonNetwork.LocalPlayer.CustomProperties.GetValueOrDefault(PropName.SLOT_NUMBER, -1);
            Vector3 playerPos = Vector3.zero;
            try
            {
                playerPos = manager.spawnPoints[slotNum].position;
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.Log(ex.Message);
            }
            
            // 직업 확인
            var charClass = PhotonNetwork.LocalPlayer.CustomProperties.GetValueOrDefault(PropName.CLASS_TYPE, CharacterClass.None);
            Debug.Log($"선택한 직업 : {charClass}");
            /*if (charClass == CharacterClass.None)
                charClass = CharacterClass.WizardFire;*/
            // 팀 확인
            TeamType team = (int)PhotonNetwork.LocalPlayer.CustomProperties[PropName.TEAM_NUMBER] == 0 ? TeamType.Left : TeamType.Right;
            InGameManager.PlayerTeam = team;
            // 커스터마이징 정보 확인
            CustomizeData data = DataManager.Instance.CurrentData;
            string json = JsonUtility.ToJson(data, true);

            // 캐릭터 생성
            GameObject localPlayerObj = PhotonNetwork.Instantiate("CustomCharacter", playerPos, Quaternion.identity); // PhotonNetwork.Instantiate는 Resources 폴더에 있는 오브젝트만을 프리팹으로 사용할 수 있다.
            localPlayerObj.name = $"Player {slotNum}";
            
            // 캐릭터 초기화
            Unit.Unit playerUnit = localPlayerObj.GetComponent<Unit.Unit>();
            playerUnit.Init(charClass, team, json);


            // 로딩 완료
            PhotonNetwork.LocalPlayer.AddProperty((PropName.LOADING_COMPLETE, true));
            
            
            token = null;
            manager.ChangeState(GameStateType.Waiting);
        }
    }
}