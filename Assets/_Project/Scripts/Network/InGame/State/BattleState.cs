using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Colosseum.Network.InGame.State
{
    public class BattleState: IGameState
    {
        private InGameManager manager = null;
        private CancellationTokenSource token = null;
        
        
        public void OnInit(InGameManager manager)
        {
            this.manager = manager;
        }
        
        public void OnEnter()
        {
            _ = CheckAsync();
        }

        public void OnExit()
        {
            token?.Cancel();
            token = null;
        }


        private async UniTask CheckAsync()
        {
            token = new();
            
            
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.RaiseEvent((byte)RaiseEventType.RoundStart, null, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
                // TODO: Master만, 양 팀 승패 판정
            }


            token = null;
        }
    }
}