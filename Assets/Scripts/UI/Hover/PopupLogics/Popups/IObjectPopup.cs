using System;
using UnityEngine;

namespace UI.Hover.PopupLogics.Popups
{
    public interface IObjectPopup
    {
        public event Action CloseButton; 
        public RectTransform DrawPopup();
        public void Redraw();
        public void Subscribe();
    }
}