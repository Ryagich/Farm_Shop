using UnityEngine;

namespace Input.Cursor
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CursorHandler
    {
        public bool IsVisible { get; private set; }

        public void ChangeCursorState()
        {
            if (IsVisible)
                HideCursor();
            else
                ShowCursor();
        }
        
        public void SetCursorState(bool state)
        {
            if (state)
                ShowCursor();
            else
                HideCursor();
        }
        
        public void ShowCursor()
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            IsVisible = true;
        }
        
        private void HideCursor()
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            IsVisible = false;
        }
    }
}