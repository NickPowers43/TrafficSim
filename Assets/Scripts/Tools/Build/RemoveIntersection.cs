using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Build
{
    public class RemoveIntersection : BuildTool
    {
        private Intersection currentHighlighted;

        public RemoveIntersection()
        {
        }

        public void Activate()
        {
            base.Activate();
        }
        public override void Deactivate()
        {
            base.Deactivate();

            if (currentHighlighted != null)
            {
                currentHighlighted.selectionSprite.SetActive(false);
                currentHighlighted = null;
            }
        }
        public override void Update()
        {
            base.Update();

            Intersection temp = GetHoveredIntersection();

            if (currentHighlighted != temp)
            {
                if (currentHighlighted != null)
                {
                    currentHighlighted.selectionSprite.SetActive(false);
                }

                currentHighlighted = temp; 
                currentHighlighted.selectionSprite.SetActive(true);
            }
        }
        public override void OnClick()
        {
            base.OnClick();

            if (currentHighlighted != null)
            {
                for (int i = 0; i < Road.Roads.Count; i++)
                {
                    if (Road.Roads[i].Source.Parent == currentHighlighted || Road.Roads[i].Destination.Parent == currentHighlighted)
                    {
                        Road.Roads[i].Disconnect();
                        Road.Roads.RemoveAt(i--);
                    }
                }

                GameObject.Destroy(currentHighlighted.gameObject);
            }
        }
    }
}
