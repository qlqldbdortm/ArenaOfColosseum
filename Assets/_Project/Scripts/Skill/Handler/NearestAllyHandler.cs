using System.Collections;
using System.Collections.Generic;
using Colosseum.Data;
using Colosseum.LifeCycle;
using Colosseum.Network.InGame;
using Colosseum.Skill.Effect;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Colosseum.Skill.Handler
{
    public class NearestAllyHandler : PosHandlerBase, IInit<HitObject>, ISpawn<SkillData>, IRelease<HitObject>
    {
        public float lifeTime = 1f;
        
        [Header("탐색 범위 설정")]
        public float searchRadius = 10f;  
        public bool includeSelf = false;

        private HitObject hitObject;
        private FixedHealEffect healEffect;

        public void OnInit(HitObject hitObject)
        {
            this.hitObject = hitObject;
            healEffect = hitObject.GetComponent<FixedHealEffect>();
        }

        public void OnSpawn(SkillData data) { }
        public void OnRelease(HitObject data) { }
        public override void SetTarget(Unit.Unit caster, Vector3 target)
        {
            if (caster == null || hitObject == null) return;

            var nearest = FindNearestAlly(caster);
            
            transform.position = nearest.transform.position;
            
            if (nearest != null)
            {
                healEffect?.OnEffect(nearest);
                _ = OnRelease();
            }
        }
        private Unit.Unit FindNearestAlly(Unit.Unit caster)
        {
            var allUnits = GameObject.FindObjectsOfType<Unit.Unit>();
            Unit.Unit nearest = null;
            float minDist = float.MaxValue;

            foreach (var unit in allUnits)
            {
                if (unit == null) continue;
                if (unit.Team != caster.Team) continue;
                if (!includeSelf && unit == caster) continue;

                float dist = Vector3.Distance(caster.transform.position, unit.transform.position);
                if (dist < minDist && dist <= searchRadius)
                {
                    minDist = dist;
                    nearest = unit;
                }
            }
            return nearest;
        }
        
        private async UniTask OnRelease()
        {
            await UniTask.WaitForSeconds(lifeTime);
            PhotonNetwork.RaiseEvent((byte)RaiseEventType.HitObjectRelease, hitObject.Uid, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
        }
    }
}
