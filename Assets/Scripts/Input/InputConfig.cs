using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Input
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "configs/Input/InputConfig")]
    public class InputConfig : ScriptableObject
    {
        [field: SerializeField] public InputActionReference PointerPosition { get; private set; }
        [field: SerializeField] public InputActionReference Click { get; private set; }
        [field: SerializeField] public InputActionReference RightClick { get; private set; }
        [field: SerializeField] public InputActionReference MoveInput { get; private set; } = null!;
        [field: SerializeField] public InputActionReference OpenGameMode { get; private set; }
        [field: SerializeField] public InputActionReference OpenRedactorMode { get; private set; }
        [field: SerializeField] public InputActionReference OpenShopMode { get; private set; }
        [field: SerializeField] public InputActionReference LeftRotate { get; private set; }
        [field: SerializeField] public InputActionReference RightRotate { get; private set; }
    }
}
