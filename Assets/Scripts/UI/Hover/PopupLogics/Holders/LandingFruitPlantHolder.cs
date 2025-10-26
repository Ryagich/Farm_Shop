using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    public class LandingFruitPlantHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text PlantName { get; private set; } = null!;
        [field: SerializeField] public TMP_Text GrowStage { get; private set; } = null!;
        [field: SerializeField] public Image GrowFill { get; private set; } = null!;
    }
}