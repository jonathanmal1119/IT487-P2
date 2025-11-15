using UnityEngine;

namespace Assets.Scripts
{
	public static class Utils
	{
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0029:Use coalesce expression", Justification = "<Pending>")]
        public static T? N<T>(this T obj) where T : Object => obj == null ? null : obj;

        public static float Round(this float num) => Mathf.Round(num);
        public static Vector3 Round(this Vector3 vec) => new(vec.x.Round(), vec.y.Round(), vec.z.Round());


        public static float SineTime(float speed) => (Mathf.Sin(Time.time * (float)speed) + 1) / 2;
        public static float SineTime(double speed) => (Mathf.Sin(Time.time * (float)speed) + 1) / 2;
    }
}