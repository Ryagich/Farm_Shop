using System;
using UI.Hover.PopupLogics.Popups;
using UnityEngine;
using VContainer;

namespace UI.Hover
{
    public class HoverTrigger : MonoBehaviour, IDisposable
    {
        public event Action Disposabled;
        public IObjectPopup ObjectPopup;
        
        [Inject]
        private void Construct(IObjectPopup objectPopup)
        {
            ObjectPopup = objectPopup;
        }
        
        private void OnDestroy()
        {
            Dispose();
        }
        
        public void Dispose()
        {
            Disposabled?.Invoke();
        }
    }
}