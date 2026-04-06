using UnityEngine;

namespace Colosseum.Skill.Handler
{
    public abstract class PosHandlerBase: MonoBehaviour
    {
        public abstract void SetTarget(Unit.Unit caster, Vector3 target);
    }
}