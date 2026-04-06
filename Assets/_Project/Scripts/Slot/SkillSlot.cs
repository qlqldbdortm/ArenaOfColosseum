using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Colosseum.Data;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colosseum.Slot
{
    public class SkillSlot : MonoBehaviour
    {
        [Header("스킬 슬롯에 들어가있는 스킬 정보")] 
        [Tooltip("슬롯 스킬")] public SkillData skillData;
        [Tooltip("슬롯 스킬 아이콘")] public Image skillIcon;
        [Tooltip("슬롯 스킬 코스트")] public TextMeshProUGUI skillCost;
        
        [Header("스킬 쿨타임 정보")]
        [Tooltip("스킬 쿨타임 표기")] public TextMeshProUGUI cooldownText;
        [Tooltip("스킬 쿨타임 표시")] public Image cooldownMask;
        [Tooltip("스킬 쿨타임")] public float CooldownTime { get; private set; } = 0;

        public bool CanUse => CooldownTime <= Time.time;
        private CancellationTokenSource token = new();

        /// <summary>
        /// 스킬을 사용하기 위한 메서드 
        /// </summary>
        public void UseSkill()
        {
            // 이미 스킬을 사용해서 쿨타임 중인 경우 사용하지 못하도록 return
            if (!CanUse)
            {
                Debug.Log("스킬 쿨타임 중...");
                return;
            }

            Cooldown(skillData.skillCoolTime + Time.time);
        }

        /// <summary>
        /// 등록된 SkillData 정보를 가지고 SkillSlot 초기화
        /// </summary>
        public void SetSkill(SkillData data)
        {
            skillData = data;
            skillIcon.sprite = skillData.skillIcon;
            skillCost.text = skillData.cost.ToString();
            cooldownText.text = CooldownTime.ToString();
        }
        
        /// <summary>
        /// CooldownAsync를 실행하기 위해서 CooldownTime에 skillData의 skillCoolTime을 넣어서 사용하는 메서드
        /// </summary>
        /// <param name="cooldown"></param>
        private void Cooldown(float cooldown)
        {
            CooldownTime = cooldown;
            _ = CooldownAsync();
        }

        /// <summary>
        /// 스킬 사용 후 쿨타임이 돌아가게 하는 메서드
        /// </summary>
        private async UniTask CooldownAsync()
        {
            cooldownMask.gameObject.SetActive(true);
            
            float startTime = Time.time;
            float remTime = CooldownTime - startTime;
            while (CooldownTime > Time.time)
            {
                await UniTask.Yield(cancellationToken: token.Token);
                
                float t = Time.time - startTime;
                float amount = 1 - (t / remTime);
                
                cooldownMask.fillAmount = amount;
                cooldownText.text = $"{CooldownTime - Time.time:F1}";
            }
            cooldownMask.gameObject.SetActive(false);
        }
    }
}