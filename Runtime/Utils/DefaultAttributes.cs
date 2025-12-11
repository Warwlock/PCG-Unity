using UnityEngine;

namespace PCG
{
    public static class DefaultAttributes
    {
        public static string Pos = "Pos";
        public static string Rot = "Rot";
        public static string Sca = "Scale";

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
