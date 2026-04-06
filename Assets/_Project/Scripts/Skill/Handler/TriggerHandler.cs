using Colosseum.Skill.Effect;
using System;
using Colosseum.Network.InGame;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Lean.Pool;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Colosseum.Skill.Handler
{
    public class TriggerHandler: MonoBehaviour
    {
        [Header("공격 피격 시 출력되는 이펙트")]
        public GameObject hitEffect;
        
        private Action<Unit.Unit> onEffect = null;
        private HitObject hitObject;
        
        private void Awake()
        {
            var fixedDamage = GetComponent<FixedDamageEffect>();
            onEffect = fixedDamage.OnEffect;
            hitObject = GetComponent<HitObject>();
        }

        private void Update()
        {
            if (hitObject.impaleCount <= 0)
            {
                PhotonNetwork.RaiseEvent((byte)RaiseEventType.HitObjectRelease, hitObject.Uid, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
            }
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Unit.Unit>(out var unit))
            {
                onEffect?.Invoke(unit);
                hitObject.impaleCount--;
            }
        }
    }
}