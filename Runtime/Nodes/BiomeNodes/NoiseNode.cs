using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace PCG.Biome
{
    [System.Serializable, NodeMenuItem("Noise", typeof(PCGBiomeGraph))]
    public class NoiseNode : BaseBiomeJobNode
    {
        [SerializeField]
        public MathOperators.NoiseFunctions noiseFunctions;

        [Input]
        public NativeArray<float3> pointsIn;

        [Input, SerializeField]
        public Vector3 frequency = new Vector3(0.1f, 0.1f, 0.1f);
        [Input, SerializeField]
        public float amplitude = 1f, lacunarity = 2f, gain = 0.5f;

        [Input, SerializeField, Range(1, 16)]
        public int octaves = 1;

        [Output]
        public NativeArray<float3> points;

        public override JobHandle Process(JobHandle dependsOn)
        {
            return default;
        }
    }
}
