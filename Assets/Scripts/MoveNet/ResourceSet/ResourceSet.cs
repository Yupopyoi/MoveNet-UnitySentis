using UnityEngine;
using Unity.Sentis;

namespace MoveNet
{
    [CreateAssetMenu(fileName = "MoveNet",
                     menuName = "ScriptableObjects/MoveNet Resource Set")]
    public sealed class ResourceSet : ScriptableObject
    {
        public ModelAsset model;
        public ComputeShader preprocess;
    }

}//namespace MoveNet
