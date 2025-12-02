using UnityEngine;

namespace PCG
{
    public static class DefaultAttributes
    {
        public static string Pos = "Pos";
        public static string PosX = "PosX";
        public static string PosY = "PosY";
        public static string PosZ = "PosZ";

        public static string Rot = "Rot";
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

        public enum AttributeType
        {
            Float, Int, Float3, Quaternion
        }
    }
}
