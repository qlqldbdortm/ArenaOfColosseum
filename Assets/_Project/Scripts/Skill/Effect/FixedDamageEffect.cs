using UnityEngine;

namespace Colosseum.Skill.Effect
{
    public class FixedDamageEffect: MonoBehaviour, IEffect
    {
        public int Damage { get; private set; } = 0;

        public void OnEffect(Unit.Unit unit)
        {
            var hitObject = GetComponentInParent<HitObject>();
            if (hitObject != null && unit == hitObject.Owner) return;
            SetInfluence(hitObject.UseSkillData.power);
            unit.TakeDamage(Damage);
        }

        public void SetInfluence(float influence)
        {
            Damage = (int)influence;
        }
    }
}