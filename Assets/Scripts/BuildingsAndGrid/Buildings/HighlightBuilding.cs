using System.Collections.Generic;
using UnityEngine;

namespace BuildingsAndGrid.Buildings
{
    public class HighlightBuilding : MonoBehaviour
    {
        [field: SerializeField] public List<GameObject> Visual { get; private set; }
        [field: SerializeField] public Transform Content { get; private set; }
        [field: SerializeField] public bool HaveLastPosition;
        [field: SerializeField] public Vector3 LastPosition;
        [field: SerializeField] public Vector3 LastLocalPosition;
        [field: SerializeField] public Quaternion LastRotation;
        public List<Tile> LastTiles = new();
        
        public void RotateLeft()
        {
            Content.Rotate(new Vector3(0,-90,0));
            var localPosition = Content.localPosition;
            Content.localPosition = new Vector3(localPosition.z, localPosition.y, localPosition.x);
        }
        
        public void RotateRight()
        {
            Content.Rotate(new Vector3(0,90,0));
            var localPosition = Content.localPosition;
            Content.localPosition = new Vector3(localPosition.z, localPosition.y, localPosition.x);
        }
        
        public Quaternion GetContentRotation() => Content.rotation;
        
        
        
        public void SetMaterial(Material material)
        {
            foreach (var visual in Visual)
            {
                visual.GetComponent<MeshRenderer>().material = material;
            }
        }
    }
}