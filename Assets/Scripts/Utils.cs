using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public static class Utils
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0029:Use coalesce expression", Justification = "<Pending>")]
        public static T? N<T>(this T obj) where T : Object => obj == null ? null : obj;

        public static float Round(this float num) => Mathf.Round(num);
        public static Vector3 Round(this Vector3 vec) => new(vec.x.Round(), vec.y.Round(), vec.z.Round());

        public static float SineTime(double speed) => SineTime((float)speed);
        public static float SineTime(float speed) => (Mathf.Sin(Time.time * (float)speed) + 1) / 2;

        public static Camera CurrentCamera => Camera.allCameras.First();

        public static float Remap(this float value, float from1, float to1, float from2, float to2) => (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        public static float Remap(this int value, float from1, float to1, float from2, float to2) => (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        
        public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);

        public static float Invert(this float value) => 1 - value;
    }
}