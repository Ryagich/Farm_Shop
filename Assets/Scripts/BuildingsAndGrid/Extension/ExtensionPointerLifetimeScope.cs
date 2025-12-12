using UI.Hover;
using UI.Hover.PopupLogics.Popups;
using VContainer;
using VContainer.Unity;

namespace BuildingsAndGrid.Extension
{
    public class ExtensionPointerLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var hoverTrigger = gameObject.AddComponent<HoverTrigger>();

            builder.RegisterInstance(hoverTrigger).AsSelf();
            
            builder.RegisterComponentInHierarchy<ExtensionPointer>();
            
            builder.Register<ExtensionPointerPopup>(Lifetime.Scoped)
                   .As<IObjectPopup>()
                   .AsSelf();
          
            builder.RegisterBuildCallback(c => { c.Inject(hoverTrigger); });
        }
    }
}