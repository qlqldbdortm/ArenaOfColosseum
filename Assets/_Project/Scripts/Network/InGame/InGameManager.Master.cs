using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Colosseum.Network.InGame
{
    public partial class InGameManager
    {
        private readonly HashSet<Player> diedPlayers = new();
        
        
        private void CheckPlayerDie(Player player)
        {
            diedPlayers.Add(player);

            if (!PhotonNetwork.IsMasterClient) return;
            
            bool leftAllDied = true;
            bool rightAllDied = true;

            foreach (var p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (!diedPlayers.Contains(p))
                {
                    if (p.CustomProperties.GetValueOrDefault(PropName.TEAM_NUMBER, TeamType.Left) == TeamType.Left)
                    {
                        leftAllDied = false;
                    }
                    else
                    {
                        rightAllDied = false;
                    }
                }
            }

            if (leftAllDied && rightAllDied)
            {
                PhotonNetwork.RaiseEvent((byte)RaiseEventType.RoundEnd, TeamType.Left, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
            }
            else if (leftAllDied)
            {
                PhotonNetwork.RaiseEvent((byte)RaiseEventType.RoundEnd, TeamType.Right, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
            }
            else if (rightAllDied)
            {
                PhotonNetwork.RaiseEvent((byte)RaiseEventType.RoundEnd, TeamType.Left, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
            }
        }
    }
}