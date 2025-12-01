using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SectionButtons : MonoBehaviour
    {
        [field: SerializeField] public Button ToShop { get; private set; }
        [field: SerializeField] public Button Garden { get; private set; }
        [field: SerializeField] public Button Production { get; private set; }
    }
}