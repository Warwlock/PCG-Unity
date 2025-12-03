using UnityEngine;

namespace PCG
{
    public static class DefaultAttributes
    {
        public static string Pos = "Pos";
        /*public static string PosX = "Pos.X";
        public static string PosY = "Pos.Y";
        public static string PosZ = "Pos.Z";*/

        public static string Rot = "Rot";
        /*public static string RotX = "Rot.X";
        public static string RotY = "Rot.Y";
        public static string RotZ = "Rot.Z";
        public static string RotW = "Rot.W";*/

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
