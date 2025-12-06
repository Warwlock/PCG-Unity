using UnityEngine;
using Unity.Jobs;

namespace PCG
{
    struct PCGEmptyJob : IJob
    {
        public void Execute() { }
    }
}
