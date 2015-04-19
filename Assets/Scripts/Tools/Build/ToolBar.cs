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
        private RemoveRoad removeRoad;
        private AddIntersection addIntersection;
        private RemoveIntersection removeIntersection;
        private AddPointOfInterest addPointOfInterest;

        public ToolBar()
        {
            addRoad = new AddRoad();
            removeRoad = new RemoveRoad();
            addIntersection = new AddIntersection();
            removeIntersection = new RemoveIntersection();
            addPointOfInterest = new AddPointOfInterest();
        }

        public void Activate(Tools tool)
        {
            switch (tool)
            {
                default:
                    throw new NotImplementedException();
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
                    throw new NotImplementedException();
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
                        ActiveTool = removeIntersection;
                    }
                    break;
                case Tools.BuildRoad:
                    if (arg)
                    {
                        ActiveTool = addRoad;
                    }
                    else
                    {
                        ActiveTool = removeRoad;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    } 
}
