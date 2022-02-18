using UnityEngine;

namespace GodControllers
{
    /// <summary>
    /// ワールドカメラ。カメラのズームと移動を行う。
    /// </summary>
    public class WorldCamera : MonoBehaviour
    {
        [SerializeField] Demo demo;
        [SerializeField] new Camera camera;
        float defaultFieldOfView;
        bool isMoving;
        GodDir moveDir;
        Vector3 moveBasePos;
        
        void Start()
        {
            moveBasePos        = transform.localPosition;
            defaultFieldOfView = camera.fieldOfView;
        }

        public void OnPinch(GodTouch t0, GodTouch t1)
        {
            // カメラズーム
            camera.fieldOfView = defaultFieldOfView * t0.PinchRate;
        }

        public void OnDoubleSwipe(GodTouch t0, GodTouch t1)
        {
            // カメラ移動
            if(!isMoving)
            {
                isMoving = true;
                moveDir  = t0.Dir;
            }

            // カメラ移動は横方向か縦方向かの平行移動のみ(地図アプリみたいに自由移動にしてもいいけど、操作が安定しないので今回はあえてこうした)
            if(moveDir == GodDir.Right || moveDir == GodDir.Left)
            {
                transform.SetLocalPositionX(moveBasePos.x + t0.DeltaPosition.x * demo.CameraSpeed);
            }
            else
            {
                transform.SetLocalPositionZ(moveBasePos.z + t0.DeltaPosition.y * demo.CameraSpeed);
            }
        }

        public void OnDoubleSwipeEnd(GodTouch t0, GodTouch t1)
        {
            // カメラ移動終了
            moveBasePos = transform.localPosition;
            isMoving    = false;
        }
    }
}