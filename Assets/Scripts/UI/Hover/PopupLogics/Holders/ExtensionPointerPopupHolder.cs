using TMPro;
using UnityEngine;

namespace UI.Hover.PopupLogics.Holders
{
    public class ExtensionPointerPopupHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Size { get; private set; }
        [field: SerializeField] public TMP_Text Price { get; private set; }
    }
}