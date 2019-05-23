using System;
using UnityEngine;
using System.Linq;
namespace Experiments.Shaders
{

    public class Ripple : MonoBehaviour
    {

        public Material rippleMat;
        [Header("set falloffs relative to eachother then use material to finetune")]
        public RippleInfo[] rippleInfos;
        ComputeBuffer rippleObjects;

        private void OnEnable()
        {
            rippleObjects = new ComputeBuffer(rippleInfos.Length, sizeof(float) * 4);
            rippleMat.SetBuffer("rippleObjects", rippleObjects);
        }

        private void OnDisable()
        {
            rippleObjects.Dispose();
        }

        private void Start()
        {
            SetMaterial();
        }

        private void Update()
        {
            SetMaterial();
        }

        void SetMaterial()
        {
            var riValues = rippleInfos.Select(i => new RippleInfoValue(i)).ToArray();
            rippleObjects.SetData(riValues);
        }
    }

    [Serializable]
    public struct RippleInfo
    {
        public Transform transform;
        [Range(0, 10)]
        public float falloff;
    }
    [Serializable]
    public struct RippleInfoValue
    {
        public Vector3 position;
        public float falloff;
        public RippleInfoValue(RippleInfo info)
        {
            position = info.transform.position;
            falloff = info.falloff;
        }
    }

}