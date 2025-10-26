using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;

namespace UI.Hover
{
    public class HoverTrigger : MonoBehaviour
    {
        public IObjectPopup ObjectPopup;
        
        [Inject]
        private void Construct(IObjectPopup objectPopup)
        {
            ObjectPopup = objectPopup;
        }
    }
}