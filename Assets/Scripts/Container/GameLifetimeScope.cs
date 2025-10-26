using Buyer;
using CameraScripts;
using Checkout;
using Gravity;
using Input;
using Interactable;
using Inventory;
using Inventory.Finance;
using Inventory.Item;
using MessagePipe;
using Messages;
using Movement;
using Objects;
using Shelf;
using Sounds;
using UI.Hover;
using UI.Hover.PopupLogics;
using UI.Hover.PopupLogics.Holders;
using Unity.AI.Navigation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container
{
    public class GameLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public InputConfig InputConfig { get; private set; } = null!;
        [field: SerializeField] public CameraConfig CameraConfig { get; private set; } = null!;
        [field: SerializeField] public PlayerMovementConfig PlayerMovementConfig { get; private set; } = null!;
        [field: SerializeField] public GravityConfig GravityConfig { get; private set; } = null!;
        [field: SerializeField] public InteractableConfig InteractableConfig { get; private set; } = null!;
        [field: SerializeField] public InventoryConfig InventoryConfig { get; private set; } = null!;
        [field: SerializeField] public ItemsConfig ItemsConfig { get; private set; } = null!;
        [field: SerializeField] public FinanceConfig FinanceConfig { get; private set; } = null!;
        [field: SerializeField] public PlayerLifetimeScope playerPrefab { get; private set; } = null!;
        [field: SerializeField] public BuyerSettings BuyerSettings { get; private set; } = null!;
        [field: SerializeField] public HoverSettings HoverSettings { get; private set; } = null!;
        [field: SerializeField] public SoundsConfig SoundsConfig { get; private set; } = null!;
        [field: SerializeField] public PopupHolders PopupHolders { get; private set; } = null!;
        [field: SerializeField] public Camera Camera { get; private set; } = null!;
        [field: SerializeField] public Transform ShoppingEnter { get; private set; } = null!;
        [field: SerializeField] public NavMeshSurface NavMeshSurface { get; private set; } = null!;
        [field: SerializeField] public Canvas Canvas { get; private set; } = null!;

        private PlayerLifetimeScope playerScope;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // === Общие зависимости ===
            builder.RegisterInstance(InputConfig).AsSelf();
            builder.RegisterInstance(CameraConfig).AsSelf();
            builder.RegisterInstance(PlayerMovementConfig).AsSelf();
            builder.RegisterInstance(GravityConfig).AsSelf();
            builder.RegisterInstance(InteractableConfig).AsSelf();
            builder.RegisterInstance(InventoryConfig).AsSelf();
            builder.RegisterInstance(ItemsConfig).AsSelf();
            builder.RegisterInstance(FinanceConfig).AsSelf();
            builder.RegisterInstance(BuyerSettings).AsSelf();
            builder.RegisterInstance(HoverSettings).AsSelf();
            builder.RegisterInstance(SoundsConfig).AsSelf();

            builder.RegisterInstance(PopupHolders).AsSelf();
            builder.RegisterInstance(NavMeshSurface).AsSelf();
            builder.RegisterInstance(Canvas).AsSelf();
            builder.RegisterInstance(ShoppingEnter).As<Transform>().Keyed("ShoppingEnter"); 
            
            builder.RegisterInstance(Camera).AsSelf();

            var soundsManager = gameObject.AddComponent<SoundsManager>();
            builder.RegisterComponent(soundsManager).AsSelf();
            
            builder.Register<ObjectCreator>(Lifetime.Singleton).AsSelf();
            builder.Register<ObjectMoverInHisPlace>(Lifetime.Singleton).AsSelf();
            builder.Register<ShelvesController>(Lifetime.Singleton).AsSelf();
            builder.Register<CheckoutsController>(Lifetime.Singleton).AsSelf();
            builder.Register<FinanceManager>(Lifetime.Singleton);

            // === MessagePipe ===
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<PlayerMoveMessage>(options);
            builder.RegisterMessageBroker<PlayerMadePurchaseMessage>(options);
            builder.RegisterMessageBroker<CreatedNewObjectMessage>(options);
            builder.RegisterMessageBroker<NewShelfCreatedMessage>(options);
            builder.RegisterMessageBroker<PlaySoundMessage>(options);
            
            // === InputHandler ===
            builder.Register<InputHandler>(Lifetime.Singleton).AsSelf().As<IStartable>();

            builder.RegisterBuildCallback(c =>
                                          {
                                              GlobalMessagePipe.SetProvider(c.AsServiceProvider());
                                              playerScope = CreateChildFromPrefab(playerPrefab, childBuilder =>
                                              {
                                                  childBuilder.RegisterMessageBroker<InteractableMessage>(options);
                                                  childBuilder.RegisterMessageBroker<InteractableEndMessage>(options);
                                              });
                                              builder.RegisterInstance(playerScope);
                                          });
            builder.UseEntryPoints(ep =>
                                   {
                                       ep.Add<ObjectCreator>();
                                       ep.Add<ObjectMoverInHisPlace>();
                                       ep.Add<HoverRaycaster>().AsSelf();
                                       ep.Add<ObjectInfoPopupsController>().AsSelf();
                                   });
            builder.RegisterBuildCallback(container =>
                                          {
                                              container.Inject(soundsManager);
                                          });
        }
    }
}
