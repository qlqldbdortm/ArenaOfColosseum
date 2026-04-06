using System;
using System.Threading;
using Colosseum.Data;
using Colosseum.LifeCycle;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Colosseum.Skill.Handler
{
    public class ProjectileHandler: PosHandlerBase, IInit<HitObject>, ISpawn<SkillData>, IRelease<HitObject>
    {
        public float moveSpeed = 5f;
        
        private Transform HitTrans => hitObject.transform;


        private CancellationTokenSource token = null;
        
        private HitObject hitObject;
        private Rigidbody rigid = null;


        void OnDestroy()
        {
            token?.Cancel();
        }

        public void OnInit(HitObject hitObject)
        {
            this.hitObject = hitObject;
            rigid = HitTrans.GetComponent<Rigidbody>();
        }

        public void OnSpawn(SkillData data)
        {
            hitObject.impaleCount = hitObject.FixedImpaleCount;
            hitObject.OnRelease().Forget();
        }
        public void OnRelease(HitObject data)
        {
            token?.Cancel();
            token = null;
        }

        public override void SetTarget(Unit.Unit caster, Vector3 target)
        {
            if (rigid == null)
            {
                _ = MoveAsync();
            }
            else
            {
                rigid.AddForce(Vector3.forward * moveSpeed, ForceMode.Impulse);
            }
        }


        private async UniTask MoveAsync()
        {
            token = new();
            while (true)
            {
                HitTrans.Translate(moveSpeed * Time.deltaTime * HitTrans.forward, Space.World);
                await UniTask.Yield(token.Token);
            }
            token = null;
        }
    }
}