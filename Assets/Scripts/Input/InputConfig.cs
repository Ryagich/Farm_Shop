using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "configs/Input/InputConfig")]
    public class InputConfig : ScriptableObject
    {
        [field: SerializeField] public InputActionReference MoveInput { get; private set; } = null!;
        [field: SerializeField] public InputActionReference PointerPosition { get; private set; }
    }
}
