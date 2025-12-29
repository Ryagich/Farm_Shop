using UnityEngine;
using UnityEngine.UI;

namespace UI.Hover.PopupLogics.Holders
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InteractableSimpleBuildingHolder : MonoBehaviour
    {
        [field: SerializeField] public Button ButtonMove { get; private set; } = null!;
        [field: SerializeField] public Button ButtonDisable { get; private set; } = null!;
    }
}