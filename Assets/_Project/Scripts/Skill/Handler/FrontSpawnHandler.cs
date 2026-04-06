using System.Collections;
using System.Collections.Generic;
using Colosseum.Data;
using Colosseum.LifeCycle;
using Colosseum.Network.InGame;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Colosseum.Skill.Handler
{
    public class FrontSpawnHandler : PosHandlerBase, IInit<HitObject>, ISpawn<SkillData>, IRelease<HitObject>
    {
        public float distanceFromPlayer = 2f;
        private HitObject hitObject;

        public void OnInit(HitObject hitObject)
        {
            this.hitObject = hitObject;
        }

        public void OnSpawn(SkillData data)
        {
            hitObject.impaleCount = hitObject.FixedImpaleCount;
            hitObject.OnRelease().Forget();
        }
        public void OnRelease(HitObject data) { }

        public override void SetTarget(Unit.Unit caster, Vector3 target)
        {
            Debug.Log(caster);
            Vector3 spawnPos = caster.transform.position + caster.transform.forward * distanceFromPlayer;
            hitObject.transform.position = spawnPos;
            hitObject.transform.rotation = caster.transform.rotation;
        }
    }
}
