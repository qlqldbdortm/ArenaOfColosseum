using Colosseum.Data;
using Colosseum.InputSystem.InputHandler;
using Colosseum.Skill;
using Colosseum.UI.InGame;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace Colosseum.Unit
{
    public partial class PlayerHandler
    {
        /// <summary>
        /// 플레이어가 공격 중에 이동이 불가하게 하기 위한 변수
        /// </summary>
        private bool IsAttack { get; set; } = false;

        private static uint HitObjectCount { get; set; } = 0;

        private Vector3 Direction => transform.forward;


        private void OnNormalAttack()
        {
            if (IsAttack || CanAct(unit.BaseData.normalAttackData)) return;
            IsAttack = true;
            unit.ConsumeStamina(unit.BaseData.normalAttackData.cost);
            
            photonView.RPC("NormalAttackRpc", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.UserId}{++HitObjectCount}",
                transform.position + Direction);
            DoBehaviourAnimation(unit.BaseData.normalAttackData.behaviourState);
        }

        [PunRPC]
        private void NormalAttackRpc(string id, Vector3 target, PhotonMessageInfo info)
        {
            float netDelay = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.LookAt(target, Vector3.up);
            _ = SpawnObjectAsync(unit.BaseData.normalAttackData, target, unit.BaseData.normalAttackData.prefabDelay - netDelay, id);
        }

        private void OnHardAttack()
        {
            if (IsAttack || CanAct(unit.BaseData.hardAttackData)) return;
            IsAttack = true;
            unit.ConsumeStamina(unit.BaseData.hardAttackData.cost);
            
            photonView.RPC("HardAttackRpc", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.UserId}{++HitObjectCount}", transform.position + Direction);
            DoBehaviourAnimation(unit.BaseData.hardAttackData.behaviourState);
        }

        [PunRPC]
        private void HardAttackRpc(string id, Vector3 target, PhotonMessageInfo info)
        {
            float netDelay = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.LookAt(target, Vector3.up);
            _ = SpawnObjectAsync(unit.BaseData.hardAttackData, target, unit.BaseData.hardAttackData.prefabDelay - netDelay, id);
        }

        private void OnSkill1()
        {
            if (PlayerUIManager.Instance is not null )
            {
                if (IsAttack || CanAct(unit.BaseData.skillDataSlots[0])) return;
                IsAttack = true;
                unit.ConsumeStamina(unit.BaseData.skillDataSlots[0].cost);
                
                photonView.RPC("OnSkill1RPC", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.UserId}{++HitObjectCount}",
                    transform.position + Direction);
                DoBehaviourAnimation(unit.BaseData.skillDataSlots[0].behaviourState);
            }
        }

        [PunRPC]
        private void OnSkill1RPC(string id, Vector3 target, PhotonMessageInfo info)
        {
            float netDelay = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.LookAt(target, Vector3.up);
            _ = SpawnObjectAsync(unit.BaseData.skillDataSlots[0], target,
                unit.BaseData.skillDataSlots[0].prefabDelay - netDelay, id);
        }

        private void OnSkill2()
        {
            if (PlayerUIManager.Instance is not null )
            {
                if (IsAttack || CanAct(unit.BaseData.skillDataSlots[1])) return;
                IsAttack = true;
                unit.ConsumeStamina(unit.BaseData.skillDataSlots[1].cost);

                photonView.RPC("OnSkill2RPC", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.UserId}{++HitObjectCount}", transform.position + Direction);
                DoBehaviourAnimation(unit.BaseData.skillDataSlots[1].behaviourState);
            }
        }
        [PunRPC]
        private void OnSkill2RPC(string id, Vector3 target, PhotonMessageInfo info)
        {
            float netDelay = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.LookAt(target, Vector3.up);
            _ = SpawnObjectAsync(unit.BaseData.skillDataSlots[1], target, unit.BaseData.skillDataSlots[1].prefabDelay - netDelay, id);
        }
        
        private void OnDefense()
        {
            if (IsAttack) return;
            IsAttack = true;
            photonView.RPC("DefenseRpc", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.UserId}{++HitObjectCount}",
                transform.position + Direction);
            DoBehaviourAnimation(unit.BaseData.defenseData.behaviourState);
        }
        
        private void OnDash()
        {
            var dir = Vector3.ProjectOnPlane(Direction, Vector3.up).normalized;
            transform.position += dir * 10f;

            if (IsAttack) return;
            IsAttack = true;
            photonView.RPC("DashRpc", RpcTarget.All, $"{PhotonNetwork.LocalPlayer.UserId}{++HitObjectCount}",
                transform.position + dir);
            DoBehaviourAnimation(unit.BaseData.dashData.behaviourState);
        }

        [PunRPC]
        private void DashRpc(string id, Vector3 target, PhotonMessageInfo info)
        {
            float netDelay = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.LookAt(target, Vector3.up);
            _ = SpawnObjectAsync(unit.BaseData.dashData, target, unit.BaseData.dashData.prefabDelay - netDelay, id);
        }

        private void OnQuickSlot(int skills)
        {
            // 스킬 슬롯이 사용가능한 상태일때만 사용할수 있도록 
            if (PlayerUIManager.Instance is not null && PlayerUIManager.Instance.keyboardSkillSlots[skills].CanUse)
            {
                if (IsAttack) return;
                IsAttack = true;
                PlayerUIManager.Instance.keyboardSkillSlots[skills].UseSkill();
                photonView.RPC("QuickSlotRpc", RpcTarget.All,
                    $"{PhotonNetwork.LocalPlayer.UserId}{++HitObjectCount}", transform.position + Direction,
                    skills);
                DoBehaviourAnimation(unit.BaseData.skillDataSlots[skills].behaviourState);
            }
        }

        [PunRPC]
        private void QuickSlotRpc(string id, Vector3 target, int skills, PhotonMessageInfo info)
        {
            float netDelay = (float)(PhotonNetwork.Time - info.SentServerTime);
            transform.LookAt(target, Vector3.up);
            _ = SpawnObjectAsync(unit.BaseData.skillDataSlots[skills], target,
                unit.BaseData.skillDataSlots[skills].prefabDelay - netDelay, id);
        }


        /// <summary>
        /// 애니메이션 이벤트<br/>
        /// 투사체 프리팹 발사 이벤트
        /// </summary>
        public void OnShoot()
        {
        }

        /// <summary>
        /// 애니메이션 이벤트 <br/>
        /// 공격 애니메이션이 끝났음을 알리는 이벤트
        /// </summary>
        public void OnEndAttack()
        {
            IsAttack = false;
        }


        private async UniTask SpawnObjectAsync(SkillData data, Vector3 target, float delay, string id)
        {
            if (delay > 0)
            {
                await UniTask.WaitForSeconds(delay);
            }

            HitObject.Create(data, unit, target, id);
        }
    }
}