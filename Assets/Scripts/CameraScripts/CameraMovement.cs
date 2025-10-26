using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using VContainer.Unity;

namespace CameraScripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CameraMovement : ITickable
    {
        private readonly Transform transform;
        private readonly Transform target;
        private readonly CameraConfig config;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public CameraMovement
            (
                Camera cam,
                Transform playerTransform,
                CameraConfig config
            )
        {
            transform = cam.transform;
            target = playerTransform;
            this.config = config;
        }

        public void Tick()
        {
            var targetCamPos = target.position + config.CameraPosition + config.CameraOffset;
            var targetRotation = Quaternion.Euler(config.CameraRotation);
            transform.position = Vector3.Lerp(
                                              transform.position,
                                              targetCamPos,
                                              config.Smoothing * Time.deltaTime
                                             );
            transform.rotation = Quaternion.Lerp(
                                                 transform.rotation,
                                                 targetRotation,
                                                 config.Smoothing * Time.deltaTime
                                                );
        }
    }
}