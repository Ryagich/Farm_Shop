using System;
using UnityEngine;

namespace Sounds
{
    [Serializable]
    public class SoundsSettings
    {
        [field: SerializeField] public AudioClip Clip;
        [field: SerializeField] public Vector3 position;
        [field: SerializeField] public Transform parent;
        [field: SerializeField] public float priority = 128f;
        [field: SerializeField] public float volume = 1f;
        [field: SerializeField] public float pitch = 1f;
        [field: SerializeField] public float stereoPan;
        [field: SerializeField] public float spatialBlend = 1f;
        [field: SerializeField] public float reverbZoneMix = 1f;
        [field: SerializeField] public float MinDistance;
        [field: SerializeField] public float MaxDistance;
        [field: SerializeField] public bool isUISound;
        [field: SerializeField] public float DistanceToPlay = 10.0f;

        public SoundsSettings(Vector3 position, Transform parent = null)
        {
            this.position = position;
            this.parent = parent;
        }
    }
}