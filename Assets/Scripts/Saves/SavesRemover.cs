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
            YG2.SaveProgress();
        }
        
        // private void Start()
        // {
        //     RemoveSaves();
        // }
        //
        // private void Update()
        // {
        //     RemoveSaves();
        // }
    }
}