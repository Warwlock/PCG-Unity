using UnityEngine;

namespace PCG
{
    public class MathOperators
    {
        public enum BasicFunctions
        {
            Add, Subtract, Multiply, Divide, _, Power, Logarithm
        }
        public enum OnePropertyFunctions
        {
            SquareRoot, InverseSquareRoot, Absolute, Exponent
        }
        public enum TrigonometricFunctions
        {
            Sine, Cosine, Tangent, _, Arcsine, Arccosine, Arctangent//, Arctan2
        }
        public enum Comparison
        {
            Minimum, Maximum, LessThan, GreaterThan
        }
        public enum Rounding
        {
            Round, Floor, Ceil
        }
        public enum Statistics
        {
            Mean, Median, Sum, Min, Max, Range, StdDev, Variance
        }

        public enum NoiseFunctions
        {
            RandomNoise, PerlinNoise, CellularNoise
        }

        public enum ConstantType
        {
            Float, Vector3, Int
        }

        public enum Compare
        {
            Greater, Less, GreaterEqual, LessEqual, Equal, NotEqual
        }

        public enum Axis
        {
            X, Y, Z, W
        }
    }
}
