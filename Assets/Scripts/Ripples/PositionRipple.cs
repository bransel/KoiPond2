using System;
using UnityEngine;
using System.Linq;
namespace KoiPond2
{

    public class PositionRipple : MonoBehaviour
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

}