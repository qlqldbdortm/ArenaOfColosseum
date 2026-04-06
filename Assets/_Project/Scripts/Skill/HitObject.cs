using System;
using System.Collections.Generic;
using Colosseum.Data;
using Colosseum.LifeCycle;
using Colosseum.Network.InGame;
using Colosseum.Skill.Handler;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Lean.Pool;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Colosseum.Skill
{
    public class HitObject: MonoBehaviour
    {
        public static Dictionary<string, HitObject> SpawnedObjects = new();

        public SkillData UseSkillData { get; private set; } = null;
        
        public bool CanEffectPlayer { get; private set; } = false;
        public string Uid { get; private set; } = string.Empty;
        public Unit.Unit Owner { get; private set; } = null;
        
        public float lifeTime = 3f;
        public int impaleCount = 1;
        public int FixedImpaleCount { get; private set; }

        private Action<Collider> onHit = null;
        private Action<SkillData> onSpawn = null;
        private Action<HitObject> onRelease = null;
        
        private PosHandlerBase posHandler = null;

        private bool hasInited = false;
        private CancellationTokenSource tokenSource;


        //void OnTriggerEnter(Collider other)
        //{
        //    onHit?.Invoke(other);
        //}
        

        private void Init()
        {
            hasInited = true;
            FixedImpaleCount = impaleCount;
            posHandler = GetComponent<PosHandlerBase>();
            
            GetComponentsInChildren<IInit<HitObject>>().GetActions()?.Invoke(this);
            onSpawn = GetComponentsInChildren<ISpawn<SkillData>>().GetActions();
            onRelease = GetComponentsInChildren<IRelease<HitObject>>().GetActions();
        }
        private void Spawn(SkillData data, Unit.Unit created, Vector3 target, string id)
        {
            SpawnedObjects.Add(id, this);
            Uid = id;
            UseSkillData = data;
            Owner = created;
            tokenSource = new CancellationTokenSource();
            onSpawn?.Invoke(data);
            
            // 현재 플레이어가 피격 대상이 맞는지 판단하는 계산식
            CanEffectPlayer = data.targetType == SkillTargetType.Everyone ||
                              (created.Team == InGameManager.PlayerTeam ^ data.targetType == SkillTargetType.Enemy);
            gameObject.layer = LayerMask.NameToLayer(CanEffectPlayer ? "AllyTarget" : "EnemyTarget");


            if (posHandler != null)
            {
                posHandler.SetTarget(created, target);
            }
        }
        public void Release()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }
            
            SpawnedObjects.Remove(Uid);
            onRelease?.Invoke(this);
            
            LeanPool.Despawn(this);
        }
        
        public static HitObject Create(SkillData data, Unit.Unit created, Vector3 target, string id)
        {
            HitObject result = LeanPool.Spawn(data.skillPrefab, created.transform.position, created.transform.rotation);
            if (!result.hasInited) result.Init();
            result.Spawn(data, created, target, id);

            return result;
        }
        public static HitObject CreateFromPrefab(GameObject prefab, SkillData data, Unit.Unit created, Vector3 target, string id)
        {
            HitObject result = LeanPool.Spawn(prefab, created.transform.position, created.transform.rotation)
                .GetComponent<HitObject>();

            if (!result.hasInited) result.Init();
            result.Spawn(data, created, target, id);

            return result;
        }
        
        [ContextMenu("Release")]
        private void ReleaseMenu()
        {
            PhotonNetwork.RaiseEvent((byte)RaiseEventType.HitObjectRelease, Uid, new RaiseEventOptions{Receivers = ReceiverGroup.All}, SendOptions.SendReliable);
        }
        
        public async UniTask OnRelease()
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(lifeTime), cancellationToken: tokenSource.Token);
                
                PhotonNetwork.RaiseEvent((byte)RaiseEventType.HitObjectRelease, Uid, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[HitObject] {Uid} OnRelease 취소됨 (풀 반환됨)");
            }
        }
    }
}