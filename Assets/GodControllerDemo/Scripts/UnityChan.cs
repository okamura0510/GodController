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
        string parameter = "";
        string motion    = "";
        string endMotion = "";
        bool isPlayingMotion;
        bool isRunning;
        bool isJumping;
        float jumpTime;
        Vector3 jumpDirPos;
        Action reservedMotion;
        
        void Update()
        {
            if(reservedMotion != null && !isJumping && endMotion == "")
            {
                // 予約モーションがある場合は即次へ
                reservedMotion();
                reservedMotion = null;
            }
            else
            {
                // ジャンプ中
                if(isJumping)
                {
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
            
                // 終了モーションがある場合は監視し、終わったら初期化する
                // モーション開始と終了をスクリプトで監視してるけど、本当は Animator 任せにした方が良い(安定しない。。)
                if(endMotion != "" && animator.GetBool(parameter))
                {
                    var state = animator.GetCurrentAnimatorStateInfo(0);
                    if(!isPlayingMotion)
                    {
                        if(state.IsName(motion)) isPlayingMotion = true;
                    }
                    else
                    {
                        if(state.IsName(endMotion)) EndMotion();
                    }
                }
            }
        }

        public void OnSwipe(GodTouch t)
        {
            // 移動
            if(parameter == "Attack")  return;
            if(parameter == "Jump")    return;
            if(isJumping)              return;
            if(reservedMotion != null) return;
            
            // 向き変更
            transform.LookAt(new Vector3(t.DeltaPosition.x, 0, t.DeltaPosition.y));

            // 歩行 or ラン
            float moveSpeed;
            if(isRunning)
            {
                StartMotion("Run");
                moveSpeed = demo.RunSpeed;
            }
            else
            {
                StartMotion("Walk");
                moveSpeed = demo.WalkSpeed;
            }
            transform.Translate(0, 0, -moveSpeed);
            transform.SetLocalPositionY(0);
        }

        public void OnSwipeEnd(GodTouch t)
        {
            // 移動終了
            EndMotion();
            StartMotion("Stand", "Standing@loop", "Standing@loop");
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
            if(parameter == "Walk") return;
            if(parameter == "Run")  return;

            jumpDirPos = new Vector3(t.DeltaPosition.x, 0, t.DeltaPosition.y);
            if(parameter == "Attack" || parameter == "Jump" || isJumping)
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
                StartMotion("Jump", "TopOfJump", "Standing@loop");
                isJumping = true;
                jumpTime  = 0;
            }
        }
        
        public void OnTap(GodTouch t)
        {
            // 攻撃
            if(parameter == "Walk") return;
            if(parameter == "Run")  return;

            if(parameter == "Attack" || parameter == "Jump" || isJumping)
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
                StartMotion("Attack", "KneelDownToUp", "Standing@loop");
            }
        }

        void StartMotion(string parameter, string motion = "", string endMotion = "")
        {
            if(this.parameter == parameter) return;

            this.parameter  = parameter;
            this.motion     = (motion    != "") ? $"Base Layer.{motion}"      : "";
            this.endMotion  = (endMotion != "") ? $"Base Layer.{endMotion}"   : "";
            isPlayingMotion = false;
            animator.SetBool(parameter, true);
        }

        void EndMotion()
        {
            animator.SetBool(parameter, false);
            parameter       = "";
            motion          = "";
            endMotion       = "";
            isPlayingMotion = false;
        }
    }
}