using UnityEngine;

namespace Purchase
{
    public class Sine : MonoBehaviour
    {
        [SerializeField] private SineConfig SineConfig;
        
        private float t;
        private Vector3 startPos, startScale;

        private void Start()
        {
            if (SineConfig.RandomizeTime)
                t = Random.Range(0, Mathf.PI * 2);
            var trans = transform;
            startPos = trans.localPosition;
            startScale = trans.localScale;
        }

        private void Update()   
        {
            t = (t + Time.deltaTime * SineConfig.Speed) % (Mathf.PI * 2);
            var k = SineConfig.Min + (Mathf.Sin(t) / 2 + 0.5f) * (SineConfig.Max - SineConfig.Min);
            transform.localPosition = startPos + SineConfig.Movement * k;
            transform.localScale = new Vector3(
                                               Mathf.Lerp(startScale.x, k, SineConfig.Scale.x),
                                               Mathf.Lerp(startScale.y, k, SineConfig.Scale.y),
                                               Mathf.Lerp(startScale.z, k, SineConfig.Scale.z));
        }
    }
}