using UnityEngine;
using System;

namespace GodControllers
{
    /// <summary>
    /// タッチデータ
    /// </summary>
    [Serializable]
    public class GodTouch
    {
        public int Index;
        public GodPhase Phase = GodPhase.None;
        public bool IsTouching;
        public int FingerId = -1;
        public Vector2 Position;
        public Vector2 StartPosition;
        public Vector2 DeltaPosition;
        public float DistanceX;
        public float DistanceY;
        public float Time;
        public float StartTime;
        public float DeltaTime;
        public GodPhase MouseRightPhase = GodPhase.None;
        public float MouseWheel;
        public GodDir Dir;
        public bool IsHolded;
        public float PinchStartDistance;
        public float PinchDeltaDistance;
        public float PinchDistance;
        public float PinchStartRate;
        public float PinchDeltaRate;
        public float PinchRate = 1;
    }

    /// <summary>
    /// タッチフェーズ(UnityEngine.TouchPhase に None を追加)
    /// </summary>
    public enum GodPhase
    {
        None       = -1,
        Began      = 0,
        Moved      = 1,
        Stationary = 2,
        Ended      = 3,
        Canceled   = 4,
    }

    /// <summary>
    /// タッチステータス
    /// </summary>
    public enum GodStatus
    {
        None,
        Down,
        Swipe,
        Hold,
        DoubleTouch,
        Pinch,
        DoubleSwipe,
    }

    /// <summary>
    /// タッチした向き(主に2Dゲーム用)
    /// </summary>
    public enum GodDir
    {
        Up,
        Right,
        Down,
        Left,
    }
}