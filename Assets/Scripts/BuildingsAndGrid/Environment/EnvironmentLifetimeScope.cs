using BuildingsAndGrid.Buildings;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BuildingsAndGrid.Environment
{
    public class EnvironmentLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public Transform Center { get; private set; } = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            var building = gameObject.AddComponent<Building>();
          
            builder.RegisterInstance(hoverTrigger);           
            builder.RegisterInstance(gameObject);
            builder.RegisterInstance(building);
            building.SetContent(Center);
            
            builder.Register<EnvironmentPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>();
            
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}