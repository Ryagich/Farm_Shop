using UnityEngine;

namespace UI.Configs
{
    [CreateAssetMenu(fileName = "Sprites Config", menuName = "configs/UI/Sprites")]
    public class SpritesConfig : ScriptableObject
    {
        [field: SerializeField] public Sprite GameIcon { get; private set; }
        [field: SerializeField] public Sprite ShopIcon { get; private set; }
        [field: SerializeField] public Sprite RedactorIcon { get; private set; }
        [field: SerializeField] public Sprite ProductionIcon { get; private set; }
        [field: SerializeField] public Sprite ToShopIcon { get; private set; }
        [field: SerializeField] public Sprite GardenIcon { get; private set; }
        
        [field: SerializeField] public Sprite LCMIcon { get; private set; }
        [field: SerializeField] public Sprite RCMIcon { get; private set; }
        [field: SerializeField] public Sprite RotateRight { get; private set; }
        [field: SerializeField] public Sprite RotateLeft { get; private set; }
    }
}