using MessagePipe;
using Messages;
using VContainer;
using VContainer.Unity;

namespace Buyer
{
    public class BuyersLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // builder.RegisterInstance(this);
            
            // === Local MessagePipe ===
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<BuyerIsOverMessage>(options);
            
            builder.Register<ShoppingListGenerator>(Lifetime.Singleton).AsSelf();
            builder.Register<BuyerSpawnPoints>(Lifetime.Singleton)
                   .AsSelf()
                   .As<IStartable>();
            // builder.RegisterEntryPoint<BuyerSpawnPoints>();
            builder.RegisterEntryPoint<BuyersSpawner>();
            // builder.RegisterEntryPoint<SurfaceController>();
        }
    }
}