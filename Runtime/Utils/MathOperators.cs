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

        public enum ChunkSizeEnum
        {
            _240 = 241, _120 = 121, _72 = 73, _48 = 49, _24 = 25, _12 = 13
        }

        public enum LODEnum
        {
            _0 = 1, _1 = 2, _2 = 4, _3 = 6, _4 = 8, _5 = 10, _6 = 12
        }
    }
}
