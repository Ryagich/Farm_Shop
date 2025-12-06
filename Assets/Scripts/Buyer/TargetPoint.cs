using UnityEngine;

namespace Buyer
{
    public class TargetPoint
    {
        public Transform Target;

        public TargetPoint(Transform transform)
        {
            Target = transform;
        }
    }
}