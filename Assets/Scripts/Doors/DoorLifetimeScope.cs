using BuildingsAndGrid.Buildings;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Doors
{
    //ReSharper disable once ClassNeverInstantiated.Global
    public class DoorLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public Transform Center { get; private set; } = null!;
        [field: SerializeField] public Transform DoorEnter { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            var building = gameObject.AddComponent<Building>();
          
            builder.RegisterInstance(hoverTrigger);           
            builder.RegisterInstance(gameObject);
            builder.RegisterInstance(building);
            builder.RegisterInstance(DoorEnter).Keyed("DoorEnter");

            building.SetContent(Center);
            
            builder.Register<EnvironmentPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>();
            
            builder.RegisterEntryPoint<DoorInfoRecorder>();

            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}