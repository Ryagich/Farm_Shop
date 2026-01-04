using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Input
{
    public static class BuildingNames
    {
        public static string GetEnglishKey(InputActionReference actionRef)
        {
            var action = actionRef.action;

            foreach (var binding in action.bindings)
            {
                if (!binding.isComposite && !binding.isPartOfComposite)
                {
                    var control = InputSystem.FindControl(binding.path);
                    if (control is KeyControl keyControl)
                    {
                        return keyControl.keyCode.ToString(); // ← ВСЕГДА EN
                    }
                }
            }

            return "?";
        }
    }
}