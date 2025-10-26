using UnityEngine;

namespace CameraScripts
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "configs/Camera/CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
         [field: SerializeField] public Vector3 CameraOffset { get; private set; } = Vector3.zero;
         [field: SerializeField] public Vector3 CameraPosition { get; private set; } = new(.0f, 7.0f, -12.5f);
         [field: SerializeField] public Vector3 CameraRotation { get; private set; } = new(40.0f, 40.0f, .0f);
         [field: SerializeField] public float Smoothing { get; private set; } = 4;
    }
}