using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Build
{
    class AddRoad : BuildTool
    {
        private Intersection startNode;

        public AddRoad()
        {
            startNode = null;
        }

        public override void Activate()
        {
            base.Activate();
        }
        public override void Deactivate()
        {
            base.Deactivate();

            if (startNode != null)
            {
                ToggleHighlight(startNode);
                startNode = null;
            }
        }
        public override void Update()
        {
            base.Update();

        }
        public override void OnClick()
        {
            base.OnClick();

            Intersection hovered = GetHoveredIntersection();

            if (hovered != null)
            {
                if (startNode != null)
                {
                    if (startNode.Degree < 4 && hovered.Degree < 4 && startNode != hovered)
                    {
                        Road road = GameObject.Instantiate(Road.Prefab).GetComponent<Road>();

                        road.SetEndPoints(startNode.GenerateInlet(), hovered.GenerateInlet());

                        Road.Roads.Add(road);
                    }

                    ToggleHighlight(startNode);
                    startNode = null;
                }
                else
                {
                    startNode = hovered;
                    ToggleHighlight(startNode);
                }
            }
        }
    }
}
