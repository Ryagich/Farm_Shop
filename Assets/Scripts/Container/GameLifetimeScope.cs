using BuildingsAndGrid;
using BuildingsAndGrid.Buildings;
using BuildingsAndGrid.Environment;
using Buyer;
using CameraScripts;
using Checkout;
using Doors;
using GameModes;
using Gravity;
using Input;
using Input.Cursor;
using Interactable;
using Inventory;
using Inventory.Finance;
using Inventory.Item;
using Localization;
using MessagePipe;
using Messages;
using Movement;
using Objects;
using Shelf;
using Sounds;
using UI.Configs;
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
        [field: SerializeField] public UIConfig UIConfig { get; private set; } = null!;
        [field: SerializeField] public GridSettings GridSettings { get; private set; } = null!;
        [field: SerializeField] public HighlightConfig HighlightConfig { get; private set; } = null!;
        [field: SerializeField] public PlayerLifetimeScope PlayerPrefab { get; private set; } = null!;
        [field: SerializeField] public BuyerSettings BuyerSettings { get; private set; } = null!;
        [field: SerializeField] public HoverSettings HoverSettings { get; private set; } = null!;
        [field: SerializeField] public SoundsConfig SoundsConfig { get; private set; } = null!;
        [field: SerializeField] public GridEnvironmentConfig GridEnvironmentConfig { get; private set; } = null!;
        [field: SerializeField] public LocalizationConfig LocalizationConfig { get; private set; } = null!;
        [field: SerializeField] public PopupHolders PopupHolders { get; private set; } = null!;
        [field: SerializeField] public Camera Camera { get; private set; } = null!;
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
            builder.RegisterInstance(UIConfig).AsSelf();
            builder.RegisterInstance(GridSettings).AsSelf();
            builder.RegisterInstance(HighlightConfig).AsSelf();
            builder.RegisterInstance(GridEnvironmentConfig).AsSelf();
            builder.RegisterInstance(LocalizationConfig).AsSelf();
            
            builder.RegisterInstance(PopupHolders).AsSelf();
            builder.RegisterInstance(Canvas).AsSelf();
            builder.RegisterInstance(Camera).AsSelf();

            // Core
            builder.Register<ShelvesController>(Lifetime.Singleton).AsSelf();
            builder.Register<DoorsController>(Lifetime.Singleton).AsSelf();
            builder.Register<CheckoutsController>(Lifetime.Singleton).AsSelf();
            builder.Register<FinanceManager>(Lifetime.Singleton);
            builder.Register<CursorHandler>(Lifetime.Singleton);
            builder.Register<CursorController>(Lifetime.Singleton);
            builder.Register<HoverRaycaster>(Lifetime.Singleton);
            builder.Register<Storage.Storage>(Lifetime.Singleton)
                   .AsSelf()
                   .As<IStartable>();
            builder.Register<TilesController>(Lifetime.Scoped);
            builder.Register<SurfaceController>(Lifetime.Singleton)
                   .AsSelf()
                   .As<IStartable>();
            
            // === MessagePipe ===
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<PlayerMoveMessage>(options);
            builder.RegisterMessageBroker<CreatedNewObjectRequest>(options);
            builder.RegisterMessageBroker<CreatedNewBuildingOnGridRequest>(options);
            builder.RegisterMessageBroker<DeleteBuildingOnGridRequest>(options);
            builder.RegisterMessageBroker<CreatedNewObjectMessage>(options);
            builder.RegisterMessageBroker<CreatedNewObjectOnGridMessage>(options);
            builder.RegisterMessageBroker<NewShelfCreatedMessage>(options);
            builder.RegisterMessageBroker<ShelfDeletedMessage>(options);
            builder.RegisterMessageBroker<PlaySoundMessage>(options);
            builder.RegisterMessageBroker<InteractableMessage>(options);
            builder.RegisterMessageBroker<InteractableEndMessage>(options);
            builder.RegisterMessageBroker<ChoseBuildingMessage>(options);
            builder.RegisterMessageBroker<AddBuildingToStorageRequest>(options);
            builder.RegisterMessageBroker<ChangeGameModeRequest>(options);
            builder.RegisterMessageBroker<GridExtendMessage>(options);
            
            // === InputHandler ===
            builder.Register<InputHandler>(Lifetime.Singleton).AsSelf().As<IStartable>();

            //Sounds
            var soundsManager = gameObject.AddComponent<SoundsManager>();
            builder.RegisterComponent(soundsManager).AsSelf();

            builder.RegisterBuildCallback(container =>
            {
                GlobalMessagePipe.SetProvider(container.AsServiceProvider());
                playerScope = CreateChildFromPrefab(PlayerPrefab, childBuilder =>
                {
                    //childBuilder.RegisterMessageBroker<InteractableMessage>(options);
                    //childBuilder.RegisterMessageBroker<InteractableEndMessage>(options);
                });
                soundsManager.SetPlayer(playerScope);
            });
            builder.RegisterEntryPoint<ObjectCreator>().AsSelf();
            builder.RegisterEntryPoint<ObjectMoverInHisPlace>().AsSelf();
            builder.RegisterEntryPoint<ObjectInfoPopupsController>().AsSelf();
            builder.RegisterEntryPoint<GameModesController>().AsSelf();
        }
    }
}
