using NaughtyAttributes;
using UnityEngine;

namespace YG
{
    public class SavesRemover : MonoBehaviour
    {
        [Button]
        public void RemoveSaves()
        {
            YG2.SetDefaultSaves();
        }
    }
}