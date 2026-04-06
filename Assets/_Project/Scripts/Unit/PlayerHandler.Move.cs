using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Colosseum.Unit
{
    public partial class PlayerHandler
    {
        private float MoveSpeed { get; set; } = 3f;
        private float DefaultSpeed => unit.BaseData?.moveSpeed ?? 3f;
        private float SprintSpeed => unit.BaseData?.sprintSpeed ?? 5f;
        
        
        private Vector3 moveVelocity = Vector3.zero;
        private CancellationTokenSource token = null;

        
        public void LookAt(Vector3 worldPosition)
        {
            transform.DOLookAt(worldPosition, 0.25f, AxisConstraint.Y);
        }

        public void LookAtDirection(Vector3 direction) => LookAt(transform.position + direction);

        
        private void OnLook(Vector3 pos)
        {
            if (moveVelocity.magnitude > 0.01f) return;
            
            LookAt(pos);
        }
        private void OnLookDir(Vector2 dir)
        {
            if (moveVelocity.magnitude > 0.01f) return;
            
            LookAtDirection(new Vector3(dir.x, 0, dir.y));
        }

        private void OnMove(Vector2 dir)
        {
            moveVelocity = new Vector3(dir.x, 0, dir.y) * MoveSpeed;
            unit.Anim.SetFloat(MoveSpeedId, moveVelocity.magnitude);
            SetMove();
        }

        private void OnSprint(bool isSprint)
        {
            MoveSpeed = isSprint && unit.CurrentStamina > 0 ? SprintSpeed : DefaultSpeed; // 상수가 아니라, 걷는 속도를 넣어야 함

            moveVelocity = moveVelocity.normalized * MoveSpeed;
            SetMove();
        }
        
        private void SetMove()
        {
            float mag = moveVelocity.magnitude;
            if (mag > 0.01f)
            {
                if (token is null)
                {
                    _ = ConsumeStaminaForSprintAsync();
                }
                ChangeMoveAnimation(mag);
                LookAtDirection(moveVelocity);
            }
            else
            {
                token?.Cancel();
                token = null;
                ChangeMoveAnimation(0);
            }
        }


        private async UniTask ConsumeStaminaForSprintAsync()
        {
            token = new();

            while (true)
            {
                float moveSpeed = moveVelocity.magnitude;
                float amount = (moveSpeed - DefaultSpeed) / (SprintSpeed - DefaultSpeed);
                if (amount > 0.05f)
                {
                    if (unit.CurrentStamina > 0)
                    {
                        unit.ConsumeStamina(amount * Time.deltaTime);
                    }
                    else
                    {
                        OnSprint(false);
                        break;
                    }
                }
                
                await UniTask.Yield(token.Token);
            }

            token = null;
        }
    }
}