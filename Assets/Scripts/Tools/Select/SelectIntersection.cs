using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            DeselectCurrentIntersection();

            Intersection hovered = GetHoveredIntersection();

            if (hovered != null)
            {
                currentHighlighted = hovered;

                currentHighlighted.ShowPathLines();
            }
        }
    }
}
