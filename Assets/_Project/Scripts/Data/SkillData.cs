using Colosseum.Skill;
using UnityEngine;
using UnityEngine.Serialization;
using AnimationState = Colosseum.Unit.AnimationState;

namespace Colosseum.Data
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/SkillData", order = 1)]
    public class SkillData : ScriptableObject
    {
        [Header("스킬 데이터")]
        
        [Tooltip("스킬 아이콘")] public Sprite skillIcon;
        [Tooltip("스킬 명")] public string skillName;
        [Tooltip("스킬 설명")] public string skillDescription;
        [Tooltip("효과 수치")] public int power;
        [Tooltip("코스트")] public int cost;
        [Tooltip("스킬 쿨타임")] public float skillCoolTime;
        [Tooltip("스킬 시전시 애니메이션 타입")] public AnimationState behaviourState;
        [Tooltip("스킬 프리팹 생성 딜레이")] public float prefabDelay;
        [Tooltip("스킬 프리팹")] public HitObject skillPrefab;
        [Tooltip("스킬 공격 대상")] public SkillTargetType targetType;
        
        private void Reset()
        {
            skillIcon = null;
            skillName = "New Skill";
            skillDescription = "New Skill Description";
            skillPrefab = null;
            behaviourState = AnimationState.None;
        }
    }
}