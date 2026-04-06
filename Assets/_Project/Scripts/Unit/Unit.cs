using System;
using Colosseum.Authentication;
using Colosseum.Customizing;
using Colosseum.Data;
using Colosseum.InputSystem;
using Colosseum.InputSystem.InputHandler;
using Colosseum.LifeCycle;
using Colosseum.Network;
using Colosseum.Network.InGame;
using Colosseum.UI;
using Colosseum.UI.InGame;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Colosseum.Unit
{
    /// <summary>
    /// 캐릭터의 이동 및 수치 처리만 해야 함
    /// </summary>
    public class Unit : MonoBehaviourPun, IPunObservable
    {
        public Animator Anim { get; private set; } = null;

        public Action<int> OnEmotion { get; set; } = null;
        public Action<float> OnHpChanged { get; set; } = null;
        public Action<float> OnStaminaChanged { get; set; } = null;

        public ClassData BaseData { get; private set; } = null;
        public TeamType Team { get; private set; } = TeamType.Left;

        public bool IsAlive { get; private set; } = false;
        public int MaxHp => BaseData?.maxHp ?? 1;
        public int CurrentHp { get; private set; } = 1;
        public float MaxStamina => BaseData?.maxStamina ?? 1;
        public float CurrentStamina { get; private set; } = 1;
        
        public string Nickname => photonView.Owner.NickName;
        
        
        public float staminaRegenRate = 5f; 
        public float regenDelay = 2f;       
        public float lastConsumeTime = 0f;
        
        public void Init(CharacterClass characterClass, TeamType teamType, string customData)
        {
            photonView.RPC("InitRpc", RpcTarget.All, (int)characterClass, (int)teamType, customData);
        }

        [PunRPC]
        private void InitRpc(int classIdx, int teamIdx, string customData)
        {
            Anim = GetComponent<Animator>();

            BaseData = ClassDataManager.GetData((CharacterClass)classIdx); // CurrentHp = MaxHp;
            Team = (TeamType)teamIdx;

            CurrentHp = MaxHp;
            CurrentStamina = MaxStamina;
            IsAlive = true;

            var cPlayer = GetComponent<CustomHandler>();
            CustomizeData jsonData = JsonUtility.FromJson<CustomizeData>(customData);
            
            // if (photonView.IsMine)
            // {
            //     DataManager.Instance.InitData(jsonData);
            // }
            //
            // cPlayer.ApplyCustomization(classIdx);
            
            cPlayer.ApplyCustomization(classIdx, jsonData);
            GetComponentsInChildren<IInit<Unit>>().GetActions()?.Invoke(this);
            if (photonView.IsMine)
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                foreach (TextFillBar hpBar in PlayerUIManager.Instance.hpBars)
                {
                    hpBar.SetValueNotTween(CurrentHp, MaxHp);
                    OnHpChanged += hpBar.ChangeValue;
                }
                
                foreach (TextFillBar staminaBar in PlayerUIManager.Instance.staminaBars)
                {
                    staminaBar.SetValueNotTween(CurrentStamina, MaxStamina);
                    OnStaminaChanged += staminaBar.ChangeValue;
                }
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("OtherPlayer");
            }
        }


        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            int newHp = CurrentHp - damage;
            if (newHp <= 0)
            {
                // 사망 처리
                if (photonView.IsMine)
                {
                    // TODO: 조작 불가능하게 막아야 함
                    photonView.RPC("DeadAnimRpc", RpcTarget.All);
                    InputManager.ChangeActionMap(ActionMapType.Lock);
                    gameObject.layer = LayerMask.NameToLayer("OtherPlayer");
                    PhotonNetwork.RaiseEvent((byte)RaiseEventType.UnitDie, null, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
                }
            }

            ChangeHp(newHp);
        }

        [PunRPC]
        private void DeadAnimRpc(PhotonMessageInfo info)
        {
            IsAlive = false;
            Anim.SetTrigger("Die");
            // Anim.SetBool("IsAlive", IsAlive);
        }
        
        public void TakeHeal(int heal)
        {
            if (!IsAlive) return;

            int newHp = CurrentHp + heal;
            newHp = newHp > MaxHp ? MaxHp : newHp;

            ChangeHp(newHp);
        }

        private void ChangeHp(int newHp)
        {
            if (newHp != CurrentHp)
            {
                photonView.RPC("ChangeHpRpc", RpcTarget.All, newHp);
            }
        }

        [PunRPC]
        private void ChangeHpRpc(int newHp, PhotonMessageInfo info)
        {
            OnHpChanged?.Invoke(CurrentHp = newHp);
        }

        public void ConsumeStamina(float stamina)
        {
            CurrentStamina -= stamina;
            OnStaminaChanged?.Invoke(CurrentStamina);
            lastConsumeTime = Time.time;
            
            if (CurrentStamina <= 0)
            {
                // TODO: Stamina 사용 Lock
            }
        }

        public void RegenStamina(float stamina)
        {
            CurrentStamina += stamina;
            OnStaminaChanged?.Invoke(CurrentStamina);

            if (CurrentStamina > 0)
            {
                // TODO: MAX랑 비교하게 해야 함
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(CurrentStamina);
            }
            else
            {
                float newStamina = (float)stream.ReceiveNext();
                if (!Mathf.Approximately(newStamina, CurrentStamina))
                {
                    CurrentStamina = newStamina;
                    OnStaminaChanged?.Invoke(CurrentStamina);
                }
            }
        }
    }
}