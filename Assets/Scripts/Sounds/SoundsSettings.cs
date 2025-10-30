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
        [field: SerializeField] public float spatialBlend = 1f;
        [field: SerializeField] public float reverbZoneMix = 1f;
        [field: SerializeField] public float MinDistance = 1f;
        [field: SerializeField] public float MaxDistance = 10;
        [field: SerializeField] public bool isUISound;
        [field: SerializeField] public float DistanceToPlay = 10.0f;

        public SoundsSettings(Vector3 position, Transform parent = null)
        {
            this.position = position;
            this.parent = parent;
        }

        public SoundsSettings(SoundsSettings oldSettings)
        {
            Clip = oldSettings.Clip;
            position = oldSettings.position;
            parent = oldSettings.parent;
            priority = oldSettings.priority;
            volume = oldSettings.volume;
            pitch = oldSettings.pitch;
            spatialBlend = oldSettings.spatialBlend;
            reverbZoneMix = oldSettings.reverbZoneMix;
            MinDistance = oldSettings.MinDistance;
            MaxDistance = oldSettings.MaxDistance;
            isUISound = oldSettings.isUISound;
            DistanceToPlay = oldSettings.DistanceToPlay;
        }
    }
}