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
            if (currentHighlighted != null)
            {
                currentHighlighted.selectionSprite.SetActive(false);
                currentHighlighted = null;
            }
        }
        public override void Update()
        {

        }
        public override void OnClick()
        {

        }
    }
}
