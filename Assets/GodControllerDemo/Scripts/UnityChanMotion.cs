using UnityEngine;

namespace GodControllers
{
    /// <summary>
    /// ユニティちゃんモーション。モーション再生や、終了モーションの検知を行っている。
    /// </summary>
    public class UnityChanMotion : StateMachineBehaviour
    {
        Animator animator;
        string parameter = "";
        string endMotion = "";

        public string Parameter => parameter;
        public string EndMotion => endMotion;

        // モーション初期化
        public void Init(Animator animator)
        {
            this.animator = animator;
        }

        // モーション再生
        public void Play(string parameter, string endMotion = "")
        {
            if(this.parameter == parameter) return;

            this.parameter = parameter;
            this.endMotion = (endMotion != "") ? $"Base Layer.{endMotion}" : "";
            animator.SetBool(parameter, true);
        }

        // モーション変化
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 終了モーションを検知したらモーションを停止させる
            if(endMotion != "" && animator.GetBool(parameter))
            {
                var state = animator.GetCurrentAnimatorStateInfo(0);
                if(state.IsName(endMotion)) Stop();
            }
        }

        // モーション停止
        public void Stop()
        {
            animator.SetBool(parameter, false);
            parameter = "";
            endMotion = "";
        }
    }
}