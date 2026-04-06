using System;
using Colosseum.Network.InGame;
using Colosseum.Skill.Effect;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Colosseum.Skill.Handler
{
    public class TriggerStayDamageHandler : MonoBehaviour
    {
        public float lifeTime = 3f;
        
        [Header("지속형 효과 간격")]
        public float tickInterval = 1f; 
        private float timer = 0f;
        
        private Action<Unit.Unit> onEffect = null;
        private HitObject hitObject;
        private void Awake()
        {
            var damageEffect = GetComponent<FixedDamageEffect>();
            onEffect = damageEffect.OnEffect;
            hitObject = GetComponent<HitObject>();
            _ = OnRelease();
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Unit.Unit>(out var unit))
            {
                onEffect?.Invoke(unit);
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<Unit.Unit>(out var unit))
            {
                timer += Time.deltaTime;
                if (timer >= tickInterval)
                {
                    timer = 0f;
                    onEffect?.Invoke(unit); 
                }
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            timer = 0f;
        }
        
        private async UniTask OnRelease()
        {
            await UniTask.WaitForSeconds(lifeTime);
            PhotonNetwork.RaiseEvent((byte)RaiseEventType.HitObjectRelease, hitObject.Uid, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
        }
    }
}