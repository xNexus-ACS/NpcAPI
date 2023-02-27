using UnityEngine;

namespace NpcAPI.Core
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this string str)
        {
            var split = str.Split(',');
            return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        }
    }
}