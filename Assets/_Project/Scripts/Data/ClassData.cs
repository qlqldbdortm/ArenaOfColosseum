using System.Collections.Generic;
using Colosseum.Unit;
using UnityEngine;

namespace Colosseum.Data
{
    [CreateAssetMenu(fileName = "ClassData", menuName = "ScriptableObjects/ClassData", order = 0)]
    public class ClassData : ScriptableObject
    {
        [Header("Unit 기초 데이터")] 
        [Tooltip("캐릭터 직업")] public CharacterClass characterClass = CharacterClass.None;
        [Tooltip("직업 아이콘")] public Sprite classIcon = null;
        [Tooltip("직업 명칭")] public string className = string.Empty;
        [Tooltip("최대 체력")] public int maxHp = 100;
        [Tooltip("최대 마력")] public int maxStamina = 20;
        [Tooltip("이동속도")] public float moveSpeed = 2f;
        [Tooltip("달리기속도")] public float sprintSpeed = 3f;
        
        [Header("Unit 공격 데이터")]
        public SkillData normalAttackData = null;
        public SkillData hardAttackData = null;
        
        [Header("Unit 스킬 데이터")]
        public List<SkillData> skillDataSlots = new List<SkillData>();

        [Header("Unit 방어 데이터")] 
        public SkillData defenseData = null;

        [Header("Unit 대시 데이터")]
        public SkillData dashData = null;
        
        private void Reset()
        {
            skillDataSlots = new List<SkillData>();
        }
    }
}