using UnityEngine;

namespace GodControllers
{
    /// <summary>
    /// 拡張メソッド
    /// </summary>
    public static class Extensions
    {
        public static void SetLocalPositionX(this Transform t, float value)
        {
            t.localPosition = new Vector3(value, t.localPosition.y, t.localPosition.z);
        }

        public static void SetLocalPositionY(this Transform t, float value)
        {
            t.localPosition = new Vector3(t.localPosition.x, value, t.localPosition.z);
        }

        public static void SetLocalPositionZ(this Transform t, float value)
        {
            t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, value);
        }

        public static void SetPositionX(this Transform t, float value)
        {
            t.position = new Vector3(value, t.position.y, t.position.z);
        }

        public static void SetPositionY(this Transform t, float value)
        {
            t.position = new Vector3(t.position.x, value, t.position.z);
        }

        public static void SetPositionZ(this Transform t, float value)
        {
            t.position = new Vector3(t.position.x, t.position.y, value);
        }

        public static void SetLocalScaleX(this Transform t, float value)
        {
            t.localScale = new Vector3(value, t.localScale.y, t.localScale.z);
        }

        public static void SetLocalScaleY(this Transform t, float value)
        {
            t.localScale = new Vector3(t.localScale.x, value, t.localScale.z);
        }

        public static void SetLocalScaleZ(this Transform t, float value)
        {
            t.localScale = new Vector3(t.localScale.x, t.localScale.y, value);
        }

        public static void SetLocalRotationX(this Transform t, float value)
        {
            t.localRotation = Quaternion.Euler(value, t.localRotation.eulerAngles.y, t.localRotation.eulerAngles.z);
        }

        public static void SetLocalRotationY(this Transform t, float value)
        {
            t.localRotation = Quaternion.Euler(t.localRotation.eulerAngles.x, value, t.localRotation.eulerAngles.z);
        }

        public static void SetLocalRotationZ(this Transform t, float value)
        {
            t.localRotation = Quaternion.Euler(t.localRotation.eulerAngles.x, t.localRotation.eulerAngles.y, value);
        }
    }
}