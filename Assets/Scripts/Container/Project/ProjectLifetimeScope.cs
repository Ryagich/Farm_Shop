using BuildingsAndGrid;
using BuildingsAndGrid.Buildings;
using BuildingsAndGrid.Environment;
using Buyer;
using CameraScripts;
using Gravity;
using Input;
using Interactable;
using Inventory;
using Inventory.Finance;
using Inventory.Item;
using Localization;
using Movement;
using Patch;
using Sounds;
using UI;
using UI.Configs;
using UI.Hover;
using UI.Hover.PopupLogics.Holders;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Container.Project
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [field: SerializeField] public InputConfig InputConfig { get; private set; }
        [field: SerializeField] public CameraConfig CameraConfig { get; private set; }
        [field: SerializeField] public PlayerMovementConfig PlayerMovementConfig { get; private set; }
        [field: SerializeField] public GravityConfig GravityConfig { get; private set; }
        [field: SerializeField] public InteractableConfig InteractableConfig { get; private set; }
        [field: SerializeField] public InventoryConfig InventoryConfig { get; private set; }
        [field: SerializeField] public ItemsConfig ItemsConfig { get; private set; }
        [field: SerializeField] public FinanceConfig FinanceConfig { get; private set; }
        [field: SerializeField] public UIConfig UIConfig { get; private set; }
        [field: SerializeField] public GridSettings GridSettings { get; private set; }
        [field: SerializeField] public HighlightConfig HighlightConfig { get; private set; }
        [field: SerializeField] public BuyerSettings BuyerSettings { get; private set; }
        [field: SerializeField] public HoverSettings HoverSettings { get; private set; }
        [field: SerializeField] public SoundsConfig SoundsConfig { get; private set; }
        [field: SerializeField] public GridEnvironmentConfig GridEnvironmentConfig { get; private set; } 
        [field: SerializeField] public LocalizationConfig LocalizationConfig { get; private set; }
        [field: SerializeField] public HelpInfoConfig HelpInfoConfig { get; private set; }
        [field: SerializeField] public SpritesConfig SpritesConfig { get; private set; }
        [field: SerializeField] public PopupHolders PopupHolders { get; private set; }
        [field: SerializeField] public Patches Patches { get; private set; }
        
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
            builder.RegisterInstance(HelpInfoConfig).AsSelf();
            builder.RegisterInstance(SpritesConfig).AsSelf();
            builder.RegisterInstance(PopupHolders).AsSelf();
            builder.RegisterInstance(Patches).AsSelf();
            
            builder.RegisterEntryPoint<Bootloader>().AsSelf();
        }
    }
}