using DG.Tweening;
using UnityEngine;

namespace Products
{
    [CreateAssetMenu(fileName = "DOAnimationConfig", menuName = "configs/DOAnimations/DOAnimationConfig")]
    public class CreatorAnimationConfig : ScriptableObject
    {
        [field: SerializeField, Min(.0f)] public float AnimationTime { get; private set; } = .5f;
        [field: SerializeField, Min(.0f)] public float ScaleFactor { get; private set; } = .5f;
        [field: SerializeField, Min(.0f)] public Ease Ease { get; private set; } = Ease.OutElastic;
        [field: SerializeField, Min(.0f)] public float Overshoot { get; private set; } = .2f;
    }
}