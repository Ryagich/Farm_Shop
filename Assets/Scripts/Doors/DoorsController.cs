using System.Collections.Generic;
using System.Linq;
using Buyer;
using UnityEngine;

namespace Doors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DoorsController
    {
        public List<TargetPoint> DoorPoints = new();

        public void RegisterDoor(Transform doorEnter)
        {
            DoorPoints.Add(new TargetPoint(doorEnter));
        }

        public void UnregisterDoor(Transform doorEnter)
        {
            var toRemove = DoorPoints.FirstOrDefault(f => f.Target == doorEnter);
            if (toRemove != null)
                DoorPoints.Remove(toRemove);
        }
    }
}