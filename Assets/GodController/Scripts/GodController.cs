using UnityEngine;
using UnityEngine.Events;

namespace GodControllers
{
    /// <summary>
    /// ゴッドコントローラー。スマホのマルチタッチに対応したバーチャルコントローラー。
    /// </summary>
    [AddComponentMenu("GodController/GodController")]
    public class GodController : MonoBehaviour
    {
        [SerializeField] GodStatus status;
        [SerializeField] float tapTime              = 0.25f;
        [SerializeField] float tapDistance          = 15;
        [SerializeField] float swipeDistanceX       = 15;
        [SerializeField] float swipeDistanceY       = 15;
        [SerializeField] bool  stopSwiping          = false;
        [SerializeField] float flickTime            = 0.25f;
        [SerializeField] float flickDistanceX       = 15;
        [SerializeField] float flickDistanceY       = 15;
        [SerializeField] float holdTime             = 0.25f;
        [SerializeField] float pinchDistance        = 20;
        [SerializeField] float pinchMin             = 0.5f;
        [SerializeField] float pinchMax             = 1;
        [SerializeField] float doubleSwipeDistanceX = 20;
        [SerializeField] float doubleSwipeDistanceY = 20;
        [SerializeField] GodTouch[] touches = { new GodTouch(){ Index = 0 }, new GodTouch(){ Index = 1 } };
        [SerializeField] UnityEvent<GodTouch> onDown;
        [SerializeField] UnityEvent<GodTouch> onUp;
        [SerializeField] UnityEvent<GodTouch> onTap;
        [SerializeField] UnityEvent<GodTouch> onSwipe;
        [SerializeField] UnityEvent<GodTouch> onSwipeEnd;
        [SerializeField] UnityEvent<GodTouch> onFlick;
        [SerializeField] UnityEvent<GodTouch> onHold;
        [SerializeField] UnityEvent<GodTouch> onHoldEnd;
        [SerializeField] UnityEvent<GodTouch, GodTouch> onPinch;
        [SerializeField] UnityEvent<GodTouch, GodTouch> onDoubleSwipe;
        [SerializeField] UnityEvent<GodTouch, GodTouch> onDoubleSwipeEnd;
        
        public GodStatus Status                                => status;
        public UnityEvent<GodTouch> OnDown                     => onDown;
        public UnityEvent<GodTouch> OnUp                       => onUp;
        public UnityEvent<GodTouch> OnTap                      => onTap;
        public UnityEvent<GodTouch> OnSwipe                    => onSwipe;
        public UnityEvent<GodTouch> OnSwipeEnd                 => onSwipeEnd;
        public UnityEvent<GodTouch> OnFlick                    => onFlick;
        public UnityEvent<GodTouch> OnHold                     => onHold;
        public UnityEvent<GodTouch> OnHoldEnd                  => onHoldEnd;
        public UnityEvent<GodTouch, GodTouch> OnPinch          => onPinch;
        public UnityEvent<GodTouch, GodTouch> OnDoubleSwipe    => onDoubleSwipe;
        public UnityEvent<GodTouch, GodTouch> OnDoubleSwipeEnd => onDoubleSwipeEnd;

        void Update()
        {
            UpdateTouchPhase(touches[0], touches[1]);
            UpdateTouchPhase(touches[1], touches[0]);

            var touchCount = 0;
            foreach(var touch in touches)
            {
                if(touch.IsTouching) touchCount++;
            }

            if(touchCount == 0)
            {
                if(status == GodStatus.DoubleSwipe) onDoubleSwipeEnd?.Invoke(touches[0], touches[1]);
                status = GodStatus.None;
            }
            else if(touchCount >= 2 || status == GodStatus.DoubleTouch || status == GodStatus.Pinch || status == GodStatus.DoubleSwipe)
            {
                if(touchCount >= 2)
                {
                    DoubleTouch(touches[0], touches[1]);
                }
            }
            else if(touchCount >= 1)
            {
                foreach(var touch in touches)
                {
                    if(touch.IsTouching) SingleTouch(touch);
                }
            }
        }

        void UpdateTouchPhase(GodTouch touch, GodTouch subTouch)
        {
            if(Application.isEditor)
            {
                // エディタ
                var pos = Input.mousePosition;

                // マウス右ボタンで強制ダブルタッチ
                if(Input.GetMouseButtonDown(1))
                {
                    touch.MouseRightPhase = GodPhase.Began;   
                }
                else if(Input.GetMouseButton(1))
                {
                    touch.MouseRightPhase = GodPhase.Moved;   
                }
                else if(Input.GetMouseButtonUp(1) || (touch.MouseRightPhase == GodPhase.Began || touch.MouseRightPhase == GodPhase.Moved)) // 低FPS時に GetMouseButtonUp が取れない問題があるようなので確実に通す
                {
                    touch.MouseRightPhase = GodPhase.Ended;
                }
                else
                {
                    touch.MouseRightPhase = GodPhase.None;
                }

                if(touch.Index == 0)
                {
                    touch.MouseWheel = 0;
                    if(touch.MouseRightPhase != GodPhase.None)
                    {
                        // ダブルタッチ(エディタでのピンチはマウスホイール量を使う)
                        touch.MouseWheel = -Input.GetAxis("Mouse ScrollWheel");
                        foreach(var t in touches)
                        {
                            t.Phase      = touch.MouseRightPhase;
                            t.IsTouching = (touch.MouseRightPhase != GodPhase.None);
                            t.Position   = pos;
                        }
                    }
                    else
                    {
                        // シングルタッチ
                        if(Input.GetMouseButtonDown(0))
                        {
                            touch.Phase = GodPhase.Began;   
                        }
                        else if(Input.GetMouseButton(0))
                        {
                            touch.Phase = GodPhase.Moved;   
                        }
                        else if(Input.GetMouseButtonUp(0) || (touch.Phase == GodPhase.Began || touch.Phase == GodPhase.Moved)) // 低FPS時に GetMouseButtonUp が取れない問題があるようなので確実に通す
                        {
                            touch.Phase = GodPhase.Ended;
                        }
                        else
                        {
                            touch.Phase = GodPhase.None;
                        }
                        touch.IsTouching = (touch.Phase != GodPhase.None);
                        touch.Position   = pos;
                    }
                }
                else
                {
                    // エディタでは2本目は特に処理しない(1本目で統一処理してる)
                    if(touch.MouseRightPhase == GodPhase.None)
                    {
                        touch.Phase      = GodPhase.None;
                        touch.IsTouching = false;
                        touch.Position   = pos;
                    }
                }
            }
            else
            {
                // 実機
                touch.Phase = GodPhase.None;
                foreach(var unityTouch in Input.touches)
                {
                    // 1本目の指が変わっても動作するように fingerId を見る
                    if(unityTouch.fingerId == subTouch.FingerId) continue;

                    if(!touch.IsTouching)
                    {
                        // タッチ開始
                        touch.IsTouching = true;
                        touch.Phase      = (GodPhase)((int)unityTouch.phase);
                        touch.FingerId   = unityTouch.fingerId;
                        touch.Position   = unityTouch.position;
                        break;
                    }
                    else if(unityTouch.fingerId == touch.FingerId)
                    {
                        // タッチ中
                        touch.Phase    = (GodPhase)((int)unityTouch.phase);
                        touch.Position = unityTouch.position;
                        break;
                    }
                }

                if(touch.Phase == GodPhase.None) 
                {
                    // タッチ終了
                    touch.IsTouching = false;
                    touch.FingerId   = -1;
                }
            }

            // タッチ開始時の更新処理
            if(touch.Phase == GodPhase.Began)
            {
                touch.StartPosition = touch.Position;
                touch.StartTime     = Time.time;
            }

            // タッチ中の更新処理
            if(touch.Phase != GodPhase.None)
            {
                touch.DeltaPosition = touch.StartPosition - touch.Position;
                touch.DistanceX     = Mathf.Abs(touch.DeltaPosition.x);
                touch.DistanceY     = Mathf.Abs(touch.DeltaPosition.y);
                touch.Time          = Time.time;
                touch.DeltaTime     = touch.Time - touch.StartTime;
            }
        }

        void SingleTouch(GodTouch touch)
        {
            if(touch.Phase == GodPhase.Began)
            {
                // 押した
                status = GodStatus.Down;
                onDown?.Invoke(touch);
            }
            else if(touch.Phase == GodPhase.Ended)
            {
                // ホールド終了
                if(touch.IsHolded)
                {
                    touch.IsHolded = false;
                    onHoldEnd?.Invoke(touch);
                }

                // スワイプ終了
                if(status == GodStatus.Swipe)
                {
                    onSwipeEnd?.Invoke(touch);
                }

                var isHorizontal = (touch.DistanceX >= flickDistanceX);
                var isVertical   = (touch.DistanceY >= flickDistanceY);
                if(touch.DeltaTime < flickTime && (isHorizontal || isVertical))
                {
                    // フリック
                    UpdateDir(touch, isHorizontal, isVertical);
                    onFlick?.Invoke(touch);
                }
                else if(touch.DeltaTime < tapTime && (touch.DistanceX < tapDistance && touch.DistanceY < tapDistance))
                {
                    // タップ
                    onTap?.Invoke(touch);
                }

                // 離した
                status = GodStatus.None;
                onUp?.Invoke(touch);
            }
            else if(touch.DeltaTime >= flickTime)
            {
                var isHorizontal = (touch.DistanceX >= swipeDistanceX);
                var isVertical   = (touch.DistanceY >= swipeDistanceY);
                if(status == GodStatus.Swipe || isHorizontal || isVertical)
                {
                    // スワイプ。通常はスワイプに移行したら動き続けるが、stopSwiping = true の場合はスワイプ中の停止が可能になる(自作ゲームの都合)
                    status = GodStatus.Swipe;
                    if(stopSwiping && !isHorizontal && !isVertical)
                    {
                        onSwipeEnd?.Invoke(touch);
                    }
                    else
                    {
                        UpdateDir(touch, isHorizontal, isVertical);
                        onSwipe?.Invoke(touch);
                    }
                }
                else if(status == GodStatus.Down || status == GodStatus.Hold)
                {
                    // ホールド
                    if(touch.DeltaTime >= holdTime)
                    {
                        status         = GodStatus.Hold;
                        touch.IsHolded = true;
                        onHold?.Invoke(touch);
                    }
                }
            }
        }
        
        void DoubleTouch(GodTouch touch0, GodTouch touch1)
        {
            if(status != GodStatus.DoubleTouch && status != GodStatus.Pinch && status != GodStatus.DoubleSwipe)
            {
                // ダブルタッチ開始
                status = GodStatus.DoubleTouch;
                touch0.PinchStartDistance = Vector2.Distance(touch0.StartPosition, touch1.StartPosition);
            }

            if(status == GodStatus.DoubleTouch)
            {
                // ダブルタッチ中
                touch0.PinchDistance      = Vector2.Distance(touch0.Position, touch1.Position);
                touch0.PinchDeltaDistance = Mathf.Abs(touch0.PinchDistance - touch0.PinchStartDistance);
                if(touch0.PinchDeltaDistance >= pinchDistance || touch0.MouseWheel != 0)
                {
                    status = GodStatus.Pinch;
                    touch0.PinchStartRate = touch0.PinchRate;
                }
                else if((touch0.DistanceX >= doubleSwipeDistanceX && touch1.DistanceX >= doubleSwipeDistanceX) || 
                        (touch0.DistanceY >= doubleSwipeDistanceY && touch1.DistanceY >= doubleSwipeDistanceY))
                {
                    status = GodStatus.DoubleSwipe;
                }
            }

            if(status == GodStatus.Pinch)
            {
                // ピンチ
                if(Application.isEditor)
                {
                    // エディタでのピンチはマウスホイール量を使う(距離判定にすると動作が安定しなかったので・・・)
                    touch0.PinchRate += touch0.MouseWheel;
                }
                else
                {
                    touch0.PinchDistance  = Vector2.Distance(touch0.Position, touch1.Position);
                    touch0.PinchDeltaRate = 1 - touch0.PinchDistance / touch0.PinchStartDistance;
                    touch0.PinchRate      = touch0.PinchStartRate + touch0.PinchDeltaRate;
                }
                touch0.PinchRate = Mathf.Clamp(touch0.PinchRate, pinchMin, pinchMax);
                onPinch?.Invoke(touch0, touch1);
            }
            else if(status == GodStatus.DoubleSwipe)
            {
                // ダブルスワイプ
                var isHorizontal = (touch0.DistanceX >= doubleSwipeDistanceX);
                var isVertical   = (touch0.DistanceY >= doubleSwipeDistanceY);
                UpdateDir(touch0, isHorizontal, isVertical);
                onDoubleSwipe?.Invoke(touch0, touch1);
            }
        }

        void UpdateDir(GodTouch touch, bool isHorizontal, bool isVertical)
        {
            if(isHorizontal && isVertical)
            {
                if(touch.DistanceX >= touch.DistanceY)
                {
                    touch.Dir = (touch.DeltaPosition.x >= 0) ? GodDir.Left : GodDir.Right;
                }
                else
                {
                    touch.Dir = (touch.DeltaPosition.y >= 0) ? GodDir.Down : GodDir.Up;
                }
            }
            else if(isHorizontal)
            {
                touch.Dir = (touch.DeltaPosition.x >= 0) ? GodDir.Left : GodDir.Right;
            }
            else if(isVertical)
            {
                touch.Dir = (touch.DeltaPosition.y >= 0) ? GodDir.Down : GodDir.Up;
            }
        }
    }
}