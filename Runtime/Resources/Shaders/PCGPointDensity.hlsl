#if UNITY_ANY_INSTANCING_ENABLED

StructuredBuffer<float> _InstanceDensityBuffer;
//uint _InstanceIDOffset;

void GetInstanceDensity_float(out float density)
{
    density = _InstanceDensityBuffer[unity_InstanceID];
}

#else

void GetInstanceDensity_float(out float density)
{
    density = 1;
}

#endif
