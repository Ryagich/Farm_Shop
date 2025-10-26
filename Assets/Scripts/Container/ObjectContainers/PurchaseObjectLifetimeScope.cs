using Purchase;
using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container.ObjectContainers
{
    public class PurchaseObjectLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public int cost { get; private set; } = 10;
        [field: SerializeField] public LifetimeScope Purchase { get; private set; }
        [field: SerializeField] public Vector3 TargetPosition { get; private set; }
        [field: SerializeField] public Vector3 TargetRotation { get; private set; }

        protected override void Configure(IContainerBuilder builder)
        {
            var interactable = gameObject.AddComponent<Interactable.Interactable>();
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();

            builder.RegisterInstance(interactable);
            builder.RegisterInstance(hoverTrigger).AsSelf();
            builder.RegisterInstance(transform);
            builder.RegisterInstance(gameObject);
          
            builder.RegisterInstance(cost).Keyed("Cost"); 
            
            builder.RegisterInstance(transform).As<Transform>().Keyed("Place"); 
            builder.RegisterInstance(Purchase).As<LifetimeScope>().Keyed("Purchase"); 
            builder.RegisterInstance(TargetPosition).As<Vector3>().Keyed("TargetPosition"); 
            builder.RegisterInstance(TargetRotation).As<Vector3>().Keyed("TargetRotation"); 

            builder.Register<PurchaseObject>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<PurchaseMoneyMover>(Lifetime.Scoped)
                   .AsSelf();
            builder.Register<PurchaseObjectPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>()
                   .AsSelf();
            
            builder.RegisterEntryPoint<PurchaseMoneyMover>().AsSelf();
            
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(hoverTrigger);
                                          });
        }
    }
}