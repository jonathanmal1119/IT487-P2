using UnityEngine;

namespace Assets.Scripts
{
	public static class Utils
	{
        public static T? N<T>(this T obj) where T : Object => obj == null ? null : obj;
        public static float Round(this float num) => Mathf.Round(num);
        public static Vector3 Round(this Vector3 vec) => new(vec.x.Round(), vec.y.Round(), vec.z);
    }
}