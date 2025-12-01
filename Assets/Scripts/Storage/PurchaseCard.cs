using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Storage
{
    public class PurchaseCard : MonoBehaviour
    {
        [field: SerializeField] public Image Icon { get; private set; } = null!;
        [field: SerializeField] public TMP_Text Name { get; private set; } = null!;
        [field: SerializeField] public Button Button { get; private set; } = null!;
        [field: SerializeField] public TMP_Text InInventory { get; private set; } = null!;
        [field: SerializeField] public TMP_Text SizeText { get; private set; } = null!;
    }
}