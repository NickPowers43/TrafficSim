using System;
using System.Collections.Generic;

namespace Tools
{
    public enum ToolArrays
    {
        Select,
        Build
    }

    public class ToolArray
    {
        protected Tool activeTool = null;
        protected Tool ActiveTool
        {
            get
            {
                return activeTool;
            }
            set
            {
                if (activeTool != null)
                {
                    activeTool.Deactivate();
                }
                activeTool = value;
                if (activeTool != null)
                {
                    activeTool.Activate();
                }
            }
        }

        public virtual void Deactivate()
        {
            if (activeTool != null)
            {
                activeTool.Deactivate();
                activeTool = null;
            }
        }
        public virtual void Update()
        {
            if (activeTool != null)
            {
                activeTool.Update();
            }
        }
        public virtual void OnClick()
        {
            if (activeTool != null)
            {
                activeTool.OnClick();
            }
        }
    } 
}
