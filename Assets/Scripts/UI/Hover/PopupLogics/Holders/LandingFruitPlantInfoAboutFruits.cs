using TMPro;
using UnityEngine;

namespace UI.Hover.PopupLogics.Holders
{
    public class LandingFruitPlantInfoAboutFruits : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text FruitsCount { get; private set; } = null!;
        [field: SerializeField] public TMP_Text FruitsReady { get; private set; } = null!;
    }
}