using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Select
{
    public class ToolBar : ToolArray
    {
        private const float MAX_HOVER_HIGHLIGHT_DIST = 32.0f;

        public enum Tools
        {
            Intersection
        }

        private Tool activeTool;
        SelectIntersection selectIntersection;

        public ToolBar()
        {

        }

        public void Activate(Tools toolToActivate)
        {
            switch (toolToActivate)
            {
                case Tools.Intersection:
                    ActiveTool = selectIntersection;
                    break;
                default:
                    break;
            }
        }
    } 
}
