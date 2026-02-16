using UnityEngine;
using UnityEngine.UI;

namespace UI.Cards
{
    public class WindowToChangeProduct : MonoBehaviour
    {
        [field: SerializeField] public Button ButtonToClose { get; private set; }
        [field: SerializeField] public RectTransform Content { get; private set; }
    }
}