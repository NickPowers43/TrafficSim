using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tools.Select
{
    class SelectIntersection : Tool
    {
        private Intersection currentHighlighted;

        public SelectIntersection()
        {
            currentHighlighted = null;
        }

        public override void Activate()
        {
            currentHighlighted = null;
        }
        public override void Deactivate()
        {
            DeselectCurrentIntersection();
        }
        public override void Update()
        {

        }

        private void DeselectCurrentIntersection()
        {
            if (currentHighlighted != null)
            {
                currentHighlighted.selectionSprite.SetActive(false);
                currentHighlighted.HidePathLines();

                currentHighlighted = null;
            }
            currentHighlighted = null;
        }

        public override void OnClick()
        {
            Intersection hovered = GetHoveredIntersection();

            if (hovered != null)
            {
                Debug.Log("hovered");

                DeselectCurrentIntersection();

                currentHighlighted = hovered;
                currentHighlighted.ShowPathLines();
            }
        }
    }
}
