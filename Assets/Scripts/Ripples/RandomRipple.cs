using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace KoiPond2
{

    public class RandomRipple : MonoBehaviour
    {

        public Material rippleMat;
        List<ClickRippleInfo> rippleInfos;
        int maxRipples = 1000;
        Plane plane;
        public Vector2Int bounds;
        [Range(0, 10)]
        public float minFrequency = 0.2f;
        [Range(0, 10)]
        public float maxFrequency = 2;
        float nextRipple;
        float lastRipple;

        private void OnEnable()
        {
            rippleInfos = new List<ClickRippleInfo>();
            plane = new Plane(Vector3.forward, transform.position);
            var ris = new Vector4[maxRipples];
            rippleMat.SetVectorArray("rippleObjects", ris);
            rippleMat.SetInt("rippleObjectCount", 0);
            rippleMat.SetFloat("timeStamp", 0);

        }


        private void Start()
        {
            SetMaterial();
            SetNextRipple();
        }

        void SetNextRipple()
        {
            nextRipple = Time.time + Random.Range(minFrequency, maxFrequency);
        }


        private void Update()
        {
            rippleMat.SetFloat("timeStamp", Time.time);
            if (Time.time > nextRipple)
            {
                var x = Random.Range(-bounds.x, bounds.x);
                var y = Random.Range(-bounds.y, bounds.y);
                var pos = new Vector3(x, y, transform.position.z);
                AddRipple(pos);
                SetNextRipple();
            }
        }

        public void AddRipple(Vector3 position)
        {
            var ri = new ClickRippleInfo();
            ri.timeStamp = Time.time;
            ri.position = position;
            rippleInfos.Add(ri);
            FilterRippleInfos();
            SetMaterial();
        }

        void FilterRippleInfos()
        {
            var duration = rippleMat.GetFloat("_Duration");
            rippleInfos = rippleInfos
            .Where(ri => (Time.time - ri.timeStamp) < duration)
            .ToList();
        }

        void SetMaterial()
        {
            int arrSize = rippleInfos.Count == 0 ? 1 : rippleInfos.Count;
            var ris = new Vector4[arrSize];
            if (rippleInfos.Count > maxRipples)
                Debug.LogWarning($"number of ripples exceeds max, please reduce ripple duration");
            for (int i = 0; i < rippleInfos.Count; i++)
            {
                ris[i] = new Vector4(rippleInfos[i].position.x, rippleInfos[i].position.y, rippleInfos[i].position.z, rippleInfos[i].timeStamp);
            }
            rippleMat.SetInt("rippleObjectCount", rippleInfos.Count);
            rippleMat.SetVectorArray("rippleObjects", ris);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            var bounds3d = new Vector3(bounds.x, bounds.y, 0);
            Gizmos.DrawWireCube(transform.position, bounds3d);
        }


        private void OnDestroy()
        {
            rippleMat.SetInt("rippleObjectCount", 0);
        }

    }

}