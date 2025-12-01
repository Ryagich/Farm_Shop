using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Storage
{
    public class ProductionCard : MonoBehaviour
    {
        [field: SerializeField] public Button Button { get; private set; } = null!;
        [field: SerializeField] public Image Icon { get; private set; } = null!;
        [field: SerializeField] public TMP_Text Text { get; private set; } = null!;
        [field: SerializeField] public TMP_Text SizeText { get; private set; } = null!;
    }
}