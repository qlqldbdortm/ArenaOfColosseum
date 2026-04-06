using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;

namespace Colosseum.Network.InGame.State
{
    public class WaitingState: IGameState
    {
        private InGameManager manager = null;
        private CancellationTokenSource token = null;
        
        
        public void OnInit(InGameManager manager)
        {
            this.manager = manager;
        }
        
        public void OnEnter()
        {
            _ = WaitingAsync();
        }

        public void OnExit()
        {
            token?.Cancel();
            token = null;
        }


        private async UniTask WaitingAsync()
        {
            token = new();
            
            
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                await UniTask.WaitUntil(() => player.CustomProperties.GetValueOrDefault(PropName.LOADING_COMPLETE, false));
            }
            
            
            token = null;
            manager.ChangeState(GameStateType.Battle);
        }
    }
}