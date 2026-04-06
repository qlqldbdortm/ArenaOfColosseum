using UnityEngine;

namespace Colosseum.Skill.Effect
{
    public class FixedHealEffect : MonoBehaviour, IEffect
    {
        public int Heal { get; private set; } = 10;
        
        public void OnEffect(Unit.Unit unit)
        {
            var hitObject = GetComponentInParent<HitObject>();
            SetInfluence(hitObject.UseSkillData.power);
            unit.TakeHeal(Heal);
        }

        public void SetInfluence(float influence)
        {
            Heal = (int)influence;
        }
    }
}
