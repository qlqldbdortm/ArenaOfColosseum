using Photon.Pun;
using UnityEngine;

namespace Colosseum.Unit
{
    public partial class PlayerHandler
    {
        private static readonly int CurrentStateId = Animator.StringToHash("CurrentState");
        private static readonly int MoveSpeedId = Animator.StringToHash("MoveSpeed");
        private static readonly int AttackId = Animator.StringToHash("Attack");
        
        
        private Animator Anim => unit.Anim;


        private int lastSpeed = 0;


        private void ChangeMoveAnimation(float speed)
        {
            int speedInt = Mathf.RoundToInt(speed);
            if (lastSpeed != speedInt)
            {
                photonView.RPC("ChangeMoveAnimationRpc", RpcTarget.All, lastSpeed = speedInt);
            }
        }
        [PunRPC]
        private void ChangeMoveAnimationRpc(int speed, PhotonMessageInfo info)
        {
            Anim.SetFloat(MoveSpeedId, speed);
        }
        
        private void DoBehaviourAnimation(AnimationState state)
        {
            photonView.RPC("DoBehaviourAnimationRpc", RpcTarget.All, (int)state);
        }
        [PunRPC]
        private void DoBehaviourAnimationRpc(int state, PhotonMessageInfo info)
        {
            Anim.SetInteger(CurrentStateId, state);
            Anim.SetTrigger(AttackId);
        }
    }
}