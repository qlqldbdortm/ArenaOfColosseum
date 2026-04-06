using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Colosseum.Data;
using Colosseum.LifeCycle;
using Colosseum.Network.InGame;
using Cysharp.Threading.Tasks;
using Lean.Pool;
using UnityEngine;

namespace Colosseum.Skill.Handler
{
    public class SpreadProjectileHandler : PosHandlerBase, IInit<HitObject>, ISpawn<SkillData>, IRelease<HitObject>
    {
        [Header("발사체 관련 설정")]
        public GameObject projectilePrefab;  
        public int arrowCount = 5;           
        public float spreadAngle = 30f;      
        public float arrowSpeed = 10f;       
        public float lifeTime = 3f;          

        private HitObject hitObject;
        private List<HitObject> spawnedArrows = new();
        
        public void OnInit(HitObject hitObject)
        {
            this.hitObject = hitObject;
        }

        public void OnSpawn(SkillData data)
        {
            _ = FireSpreadArrows(data);
        }

        public void OnRelease(HitObject data)
        {
            // foreach (var arrow in spawnedArrows)
            // {
            //     if (arrow != null && HitObject.SpawnedObjects.ContainsKey(arrow.Uid))
            //     {
            //         arrow.Release();
            //     }
            // }
            // spawnedArrows.Clear();
        }

        public override void SetTarget(Unit.Unit caster, Vector3 target)
        {

        }

        private async UniTask FireSpreadArrows(SkillData data)
        {
            var owner = hitObject.Owner;
            Vector3 origin = owner.transform.position + Vector3.up * 1f;
            Quaternion baseRotation = owner.transform.rotation;

            float startAngle = -spreadAngle / 2f;
            float angleStep = spreadAngle / (arrowCount - 1);

            for (int i = 0; i < arrowCount; i++)
            {
                float angle = startAngle + angleStep * i;
                Quaternion rot = baseRotation * Quaternion.Euler(0, angle, 0);

   
                string id = Guid.NewGuid().ToString();


                HitObject arrow = HitObject.CreateFromPrefab(projectilePrefab, data, owner, origin, id);
                arrow.transform.rotation = rot;
                spawnedArrows.Add(arrow);

                var rigid = arrow.GetComponent<Rigidbody>();
                if (rigid != null)
                {
                    rigid.velocity = rot * Vector3.forward * arrowSpeed;
                }
                else
                {
                    MoveArrowAsync(arrow, rot * Vector3.forward).Forget();
                }
                
            }
            hitObject.Release();
        }

        private async UniTaskVoid MoveArrowAsync(HitObject arrow, Vector3 direction)
        {
            float elapsed = 0f;
            while (elapsed < lifeTime && arrow != null)
            {
                arrow.transform.Translate(direction * arrowSpeed * Time.deltaTime, Space.World);
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        private async UniTask DestroyAfterLifeTime(HitObject arrow, float time)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            if (arrow != null)
                arrow.Release();
        }
    }
}