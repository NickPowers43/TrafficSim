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

        private Tool activeTool = null;
        private GameObject buildToolCursor;
        private GameObject intersectionPrefab;
        //Tool objects
        private BuildRoad buildRoad;
        private AddIntersection addIntersection;
        private AddPointOfInterest addPointOfInterest;

        public ToolBar(GameObject buildToolCursorPrefab, GameObject roadPrefab, GameObject intersectionPrefab)
        {
            this.intersectionPrefab = intersectionPrefab;

            buildToolCursor = GameObject.Instantiate(buildToolCursorPrefab);
            buildToolCursor.SetActive(false);

            buildRoad = new BuildRoad(roadPrefab);
            addIntersection = new AddIntersection(intersectionPrefab, buildToolCursor);
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
                    ActiveTool = buildRoad;
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
