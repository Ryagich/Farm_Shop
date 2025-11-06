using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using VContainer.Unity;

namespace CameraScripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CameraMovement : ITickable
    {
        private readonly CameraConfig config;
        private readonly Transform transform;
      
        private Transform target;

        public CameraMovement
            (
                Camera cam,
                Transform target,
                CameraConfig config
            )
        {
            this.config = config;
            this.target = target;
            transform = cam.transform;
        }

        public void Tick()
        {
            if (!target)
                return;
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

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public void ChangeTarget(Transform target)
        {
            this.target = target;
        }
    }
}