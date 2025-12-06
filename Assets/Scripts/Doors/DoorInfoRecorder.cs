using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Doors
{
    //ReSharper disable once ClassNeverInstantiated.Global
    public class DoorInfoRecorder : IStartable, IDisposable
    {
        private readonly DoorsController doorsController;
        private readonly Transform doorEnter;

        public DoorInfoRecorder
            (
                DoorsController doorsController,
                [Key("DoorEnter")] Transform doorEnter
            )
        {
            this.doorsController = doorsController;
            this.doorEnter = doorEnter;
        }
        
        public void Start()
        {
            doorsController.RegisterDoor(doorEnter);
        }
        
        public void Dispose()
        {
            doorsController.UnregisterDoor(doorEnter);
        }
    }
}