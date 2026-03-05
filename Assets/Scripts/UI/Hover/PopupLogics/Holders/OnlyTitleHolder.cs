using TMPro;
using UnityEngine;

namespace UI.Hover.PopupLogics.Holders
{
    public class OnlyTitleHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text Text { get; private set; }
    }
}