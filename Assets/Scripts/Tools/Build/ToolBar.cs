using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Build;

namespace Tools.Build
{
    public class ToolBar : ToolArray
    {
        public const float INTERSECTION_Z = 0.0f;

        public enum Tools
        {
            BuildPointOfInterest,
            BuildIntersection,
            BuildRoad
        }

        //Tool objects
        private AddRoad addRoad;
        private AddIntersection addIntersection;
        private AddPointOfInterest addPointOfInterest;

        public ToolBar()
        {
            addRoad = new AddRoad();
            addIntersection = new AddIntersection();
            addPointOfInterest = new AddPointOfInterest();
        }

        public void Activate(Tools tool)
        {
            switch (tool)
            {
                case Tools.BuildPointOfInterest:
                    ActiveTool = addPointOfInterest;
                    break;
                case Tools.BuildIntersection:
                    ActiveTool = addIntersection;
                    break;
                case Tools.BuildRoad:
                    ActiveTool = addRoad;
                    break;
                default:
                    break;
            }
        }
        public void Activate(Tools tool, int arg)
        {
            switch (tool)
            {
                case Tools.BuildPointOfInterest:
                    if ((PointsOfInterest)arg != PointsOfInterest.None)
                    {
                        addPointOfInterest.POI = (PointsOfInterest)arg;
                        ActiveTool = addPointOfInterest;
                    }
                    else
                    {

                    }
                    break;
                default:
                    break;
            }
        }
        public void Activate(Tools tool, bool arg)
        {
            switch (tool)
            {
                case Tools.BuildIntersection:
                    if (arg)
                    {
                        ActiveTool = addIntersection;
                    }
                    else
                    {

                    }
                    break;
                default:
                    break;
            }
        }
    } 
}
