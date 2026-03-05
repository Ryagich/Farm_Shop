using BuildingsAndGrid.Buildings;
using CameraScripts;
using Interactable;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BulletinBoard
{
    public class BulletinBoardLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public Transform Center { get; private set; }
        [field: SerializeField] public Transform CameraPosition { get; private set; }
        [field: SerializeField] public GameObject UICollider { get; private set; }
        [field: SerializeField] public GameObject ExitSign { get; private set; }
        [field: SerializeField] public GameObject PatchInfoSign { get; private set; }
        [field: SerializeField] public GameObject ExitUIZone { get; private set; }
        [field: SerializeField] public GameObject PatchUIZone { get; private set; }
        
        protected override void Configure(IContainerBuilder builder)
        {
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();
            var building = gameObject.AddComponent<Building>();

            var exitInteractable = ExitSign.AddComponent<HoverTrigger>();
            var patchInfoInteractable = PatchInfoSign.AddComponent<HoverTrigger>();

            interactable.InteractionMode = InteractionMode.Manual;
            building.SetContent(Center);

            builder.RegisterInstance(UICollider).Keyed($"UI Zone");
            builder.RegisterInstance(exitInteractable).Keyed($"ExitSign");
            builder.RegisterInstance(patchInfoInteractable).Keyed($"PatchInfoSign");
            builder.RegisterInstance(ExitUIZone).Keyed($"Exit UI Zone");
            builder.RegisterInstance(PatchUIZone).Keyed($"PatchInfo UI Zone");

            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger);
            builder.RegisterInstance(building);
            builder.RegisterInstance(CameraPosition).Keyed($"CameraPosition");

            builder.Register<BuildingInteractableFlag>(Lifetime.Scoped).AsSelf();
            builder.Register<InteractableSimpleBuildingPopup>(Lifetime.Scoped).As<IObjectPopup>().AsSelf();
            
            builder.RegisterEntryPoint<BulletinBoardInteractableLogic>().AsSelf();

            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}