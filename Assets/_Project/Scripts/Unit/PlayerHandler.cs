using Colosseum.Data;
using Colosseum.InputSystem.InputHandler;
using Colosseum.LifeCycle;
using Colosseum.UI.InGame;
using Photon.Pun;
using UnityEngine;

namespace Colosseum.Unit
{
    [RequireComponent(typeof(Unit))]
    public partial class PlayerHandler: MonoBehaviourPun, IInit<Unit>
    {
        private Unit unit = null;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        public void OnInit(Unit unit)
        {
            this.unit = unit;

            MoveSpeed = DefaultSpeed;

            if (photonView.IsMine)
            {
                // PlayerUIManager에 넣어둔 스킬 슬롯에 스킬을 세팅
                PlayerUIManager.Instance.keyboardSkillSlots[0].SetSkill(unit.BaseData.skillDataSlots[0]);
                PlayerUIManager.Instance.keyboardSkillSlots[1].SetSkill(unit.BaseData.skillDataSlots[1]);
                PlayerUIManager.Instance.keyboardSkillSlots[2].SetSkill(unit.BaseData.normalAttackData);
                PlayerUIManager.Instance.keyboardSkillSlots[3].SetSkill(unit.BaseData.hardAttackData);
                
                PlayerUIManager.Instance.padSkillSlots[0].SetSkill(unit.BaseData.hardAttackData);
                PlayerUIManager.Instance.padSkillSlots[1].SetSkill(unit.BaseData.normalAttackData);
                PlayerUIManager.Instance.padSkillSlots[2].SetSkill(unit.BaseData.skillDataSlots[1]);
                PlayerUIManager.Instance.padSkillSlots[3].SetSkill(unit.BaseData.skillDataSlots[0]);
                
                
                PlayerEventBus.OnMove += OnMove;
                PlayerEventBus.OnLook += OnLook;
                PlayerEventBus.OnLookDir += OnLookDir;
                PlayerEventBus.OnSprint += OnSprint;
                PlayerEventBus.OnNormalAttack += OnNormalAttack;
                PlayerEventBus.OnHardAttack += OnHardAttack;
                PlayerEventBus.OnSkill1 += OnSkill1;
                PlayerEventBus.OnSkill2 += OnSkill2;
                PlayerEventBus.OnEmote += OnEmote;
            }
        }
        
        void Update()
        {
            if (!photonView.IsMine) return; // 플레이어가 아닌 이상 직접 이동이 실행돼선 안 됨
            if (IsAttack) return; // 플레이어가 공격 중에는 움직이면 안됨 
            transform.Translate(moveVelocity * Time.deltaTime, Space.World);
            
            // 스테미나가 Max보다 작고, 일정 시간 지나면 회복
            if (unit.CurrentStamina < unit.MaxStamina && Time.time - unit.lastConsumeTime > unit.regenDelay)
            {
                unit.RegenStamina(unit.staminaRegenRate * Time.deltaTime);
            }
        }

        /// <summary>
        /// 공격 등 행동에 필요한 스테미나를 판단해서 bool로 반환하는 메서드 
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        private bool CanAct(SkillData skill)
        {
            return !(unit.CurrentStamina > skill.cost);
        }
    }
}