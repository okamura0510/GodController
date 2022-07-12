using UnityEngine;
using System;

namespace GodControllers
{
    /// <summary>
    /// ユニティちゃん(可愛い)。歩行、ラン、ジャンプ、攻撃をする。
    /// </summary>
    public class UnityChan : MonoBehaviour
    {
        [SerializeField] Demo demo;
        [SerializeField] Animator animator;
        UnityChanMotion motion;
        bool isRunning;
        bool isJumping;
        float jumpTime;
        Vector3 jumpDirPos;
        Action reservedMotion;
        
        void Start()
        {
            motion = animator.GetBehaviour<UnityChanMotion>();
            motion.Init(animator);
        }

        void Update()
        {
            if(reservedMotion != null && !isJumping && motion.EndMotion == "")
            {
                // 予約モーションがある場合は即次へ
                reservedMotion();
                reservedMotion = null;
            }
            else if(isJumping)
            {
                // ジャンプ中
                jumpTime += Time.deltaTime;
                if(jumpTime >= demo.JumpTime)
                {
                    transform.SetLocalPositionY(0);
                    isJumping = false;
                }
                else
                {
                    // 移動しながら高さを手動で変える
                    var rate = jumpTime / demo.JumpTime;
                    transform.Translate(0, 0, -demo.JumpSpeed);
                    transform.SetLocalPositionY(demo.JumpHeight * Mathf.Sin(Mathf.PI * rate));
                }
            }
        }

        public void OnSwipe(GodTouch t)
        {
            // 移動
            if(motion.Parameter == "Attack") return;
            if(motion.Parameter == "Jump")   return;
            if(isJumping)                    return;
            if(reservedMotion != null)       return;
            
            // 向き変更
            transform.LookAt(new Vector3(t.DeltaPosition.x, 0, t.DeltaPosition.y));

            // 歩行 or ラン
            float moveSpeed;
            if(isRunning)
            {
                motion.Play("Run");
                moveSpeed = demo.RunSpeed;
            }
            else
            {
                motion.Play("Walk");
                moveSpeed = demo.WalkSpeed;
            }
            transform.Translate(0, 0, -moveSpeed);
            transform.SetLocalPositionY(0);
        }

        public void OnSwipeEnd(GodTouch t)
        {
            // 移動終了
            motion.Stop();
            motion.Play("Stand", "Standing@loop");
        }

        public void OnHold(GodTouch t)
        {
            // ラン(移動が歩行からランに変化)
            isRunning = true;
        }

        public void OnHoldEnd(GodTouch t)
        {
            // ラン終了
            isRunning = false;
        }

        public void OnFlick(GodTouch t)
        {
            // ジャンプ
            if(motion.Parameter == "Walk") return;
            if(motion.Parameter == "Run")  return;

            jumpDirPos = new Vector3(t.DeltaPosition.x, 0, t.DeltaPosition.y);
            if(motion.Parameter == "Attack" || motion.Parameter == "Jump" || isJumping)
            {
                // 次のモーションを予約する(モーションをシームレスに繋げるため)
                reservedMotion = Jump;
            }
            else
            {
                Jump();
            }

            void Jump()
            {
                transform.LookAt(jumpDirPos);
                motion.Play("Jump", "Standing@loop");
                isJumping = true;
                jumpTime  = 0;
            }
        }
        
        public void OnTap(GodTouch t)
        {
            // 攻撃
            if(motion.Parameter == "Walk") return;
            if(motion.Parameter == "Run")  return;

            if(motion.Parameter == "Attack" || motion.Parameter == "Jump" || isJumping)
            {
                // 次のモーションを予約する(モーションをシームレスに繋げるため)
                reservedMotion = Attack;
            }
            else
            {
                Attack();
            }

            void Attack()
            {
                motion.Play("Attack", "Standing@loop");
            }
        }
    }
}