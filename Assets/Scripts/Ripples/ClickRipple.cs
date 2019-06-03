using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace KoiPond2
{

    public class ClickRipple : MonoBehaviour
    {

        public Material clickRippleMat;
        List<ClickRippleInfo> rippleInfos;
        // [Range(1, 512)]
        int maxRipples = 1000;
        // Vector3 debugPos;
        Plane plane;

        private void OnEnable()
        {
            rippleInfos = new List<ClickRippleInfo>();
            plane = new Plane(Vector3.forward, transform.position);
            var ris = new Vector4[maxRipples];
            clickRippleMat.SetVectorArray("rippleObjects", ris);
        }

        private void Start()
        {
            SetMaterial();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var enter = 0f;
                if (!plane.Raycast(ray, out enter))
                    return;
                var pos = ray.GetPoint(enter);
                // debugPos = pos;
                AddRipple(pos);
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
            var duration = clickRippleMat.GetFloat("_Duration");
            rippleInfos = rippleInfos
            .Where(ri => (Time.time - ri.timeStamp) < duration)
            .ToList();
        }

        void SetMaterial()
        {
            int arrSize = rippleInfos.Count == 0 ? 1 : rippleInfos.Count;
            var ris = new Vector4[arrSize];
            if (rippleInfos.Count > maxRipples)
                Debug.LogWarning($"number of ripples exceeds max, please reduce ripple duration or increase max ripple count");
            // Debug.Log($"setting {rippleInfos.Count} ripples");
            for (int i = 0; i < rippleInfos.Count; i++)
            {
                ris[i] = new Vector4(rippleInfos[i].position.x, rippleInfos[i].position.y, rippleInfos[i].position.z, rippleInfos[i].timeStamp);
            }
            clickRippleMat.SetInt("rippleObjectCount", rippleInfos.Count);
            clickRippleMat.SetVectorArray("rippleObjects", ris);
            // foreach (var ri in ris)
            // {
            //     Debug.Log(ri.timeStamp);
            // }
            // rippleObjects.SetData(ris);
            // clickRippleMat.SetBuffer("rippleObjects", rippleObjects);

        }

        private void OnDestroy()
        {
            clickRippleMat.SetInt("rippleObjectCount", 0);
            // clickRippleMat.SetVectorArray("rippleObjects", ris);
        }

        private void OnDrawGizmos()
        {
            // Gizmos.DrawLine(Camera.main.transform.position, debugPos);
        }

    }

}