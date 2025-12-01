using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    public class EnvironmentHolder : MonoBehaviour
    {
        [field: SerializeField] public Button ButtonMove { get; private set; } = null!;
    }
}