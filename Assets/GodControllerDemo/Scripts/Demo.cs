using UnityEngine;
using UnityEngine.UI;

namespace GodControllers
{
    /// <summary>
    /// デモシーン。ゲームデータを操作しやすいように一元管理してる。
    /// </summary>
    public class Demo : MonoBehaviour
    {
        [SerializeField] GodController controller;
        [SerializeField] Text controllerStatus;
        [SerializeField] float cameraSpeed = 0.01f;
        [SerializeField] float walkSpeed   = 0.1f;
        [SerializeField] float runSpeed    = 0.2f;
        [SerializeField] float jumpTime    = 0.6f;
        [SerializeField] float jumpSpeed   = 0.2f;
        [SerializeField] float jumpHeight  = 2;
        
        public float CameraSpeed => cameraSpeed;
        public float WalkSpeed   => walkSpeed;
        public float RunSpeed    => runSpeed;
        public float JumpTime    => jumpTime;
        public float JumpSpeed   => jumpSpeed;
        public float JumpHeight  => jumpHeight;
        
        void Update() => controllerStatus.text = controller.Status.ToString();
    }
}