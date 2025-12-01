using UnityEngine;

namespace PCG
{
    public static class DefaultAttributes
    {
        public static string PosX = "PosX";
        public static string PosY = "PosY";
        public static string PosZ = "PosZ";

        public static string RotX = "RotX";
        public static string RotY = "RotY";
        public static string RotZ = "RotZ";
        public static string RotW = "RotW";

        public static string Density = "Density";

        public static string LastModifiedAttribute = "@Last";

        public enum DefaultAttributesEnum
        {
            PosX, PosY, PosZ, RotX, RotY, RotZ, RotW, Density
        }
    }
}
