using UnityEngine;

namespace Landings
{
    public interface IGrower
    {
        public void StartGrow();
        public GameObject GivePlant();
        public void DeletePlant();
    }
}