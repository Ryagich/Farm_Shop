using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    public class LandingPlantIsItemHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text PlantName { get; private set; } = null!;
        [field: SerializeField] public TMP_Text GrowStage { get; private set; } = null!;
        [field: SerializeField] public Image GrowFill { get; private set; } = null!;
        [field: SerializeField] public Image Icon { get; private set; } = null!;
        [field: SerializeField] public Button ChangePlantButton { get; private set; } = null!;
        [field: SerializeField] public Button ButtonMove { get; private set; } = null!;
        [field: SerializeField] public Button ButtonMoveToInventory { get; private set; } = null!;
    }
}