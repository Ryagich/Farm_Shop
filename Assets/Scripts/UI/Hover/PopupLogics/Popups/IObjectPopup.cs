using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Hover.PopupLogics.Popups
{
    public interface IObjectPopup
    {
        public event Action CloseButton; 
        public RectTransform Root { get; }
        public List<RectTransform> Children { get; }
        public IObjectPopup DrawPopup(Canvas canvas);
        public void Redraw();
        public void Subscribe();
    }
}