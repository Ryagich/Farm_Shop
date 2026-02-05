using BuildingsAndGrid;
using Buyer;
using Checkout;
using Doors;
using GameModes;
using Input;
using Input.Cursor;
using Inventory.Finance;
using MessagePipe;
using Messages;
using Objects;
using Shelf;
using Sounds;
using UI.Hover;
using UI.Hover.PopupLogics;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container
{
    public class GameLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public Camera Camera { get; private set; } = null!;
        [field: SerializeField] public Canvas Canvas { get; private set; } = null!;
        [field: SerializeField] public PlayerLifetimeScope PlayerPrefab { get; private set; } = null!;
      
        private PlayerLifetimeScope playerScope;

        protected override void Configure(IContainerBuilder builder)
        {
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
           
            builder.Register<TilesController>(Lifetime.Scoped);
            builder.Register<SurfaceController>(Lifetime.Singleton)
                   .AsSelf()
                   .As<IStartable>();
            
            //Buyers
            builder.Register<ShoppingListGenerator>(Lifetime.Singleton).AsSelf();
            builder.Register<BuyerSpawnPoints>(Lifetime.Singleton)
                   .AsSelf()
                   .As<IStartable>();
            builder.RegisterEntryPoint<BuyersSpawner>().AsSelf();

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
            builder.RegisterMessageBroker<BuyerIsOverMessage>(options);
            
            // === InputHandler ===
            builder.Register<InputHandler>(Lifetime.Singleton).AsSelf().As<IStartable>();

            //Sounds
            var soundsManager = gameObject.AddComponent<SoundsManager>();
            builder.RegisterComponent(soundsManager).AsSelf();

            builder.RegisterBuildCallback(container =>
                                          {
                                              GlobalMessagePipe.SetProvider(container.AsServiceProvider());
                                              playerScope = CreateChildFromPrefab(PlayerPrefab, _ => { });
                                              soundsManager.SetPlayer(playerScope);
                                          });

            builder.RegisterEntryPoint<ObjectCreator>().AsSelf();
            builder.RegisterEntryPoint<ObjectMoverInHisPlace>().AsSelf();
            builder.RegisterEntryPoint<ObjectInfoPopupsController>().AsSelf();
            builder.RegisterEntryPoint<GameModesController>().AsSelf();
            builder.RegisterEntryPoint<Storage.Storage>().AsSelf();
        }
    }
}