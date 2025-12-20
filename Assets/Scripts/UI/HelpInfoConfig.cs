using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "HelpInfo", menuName = "configs/UI/HelpInfoConfig")]
    public class HelpInfoConfig : ScriptableObject
    {
        [field: SerializeField] public RectTransform RightSimpleHelp { get; private set; }

        [field: SerializeField] public Vector2 RightHelpDefaultPosition { get; private set; } = new (-10.0f, 10.0f);
        [field: SerializeField] public Vector3 RightHelpDistance { get; private set; }
    }
}