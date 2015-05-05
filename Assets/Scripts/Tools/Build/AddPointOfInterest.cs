using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Build
{
    public class AddPointOfInterest : Tool
    {
        private PointsOfInterest poi;
        public PointsOfInterest POI
        {
            set
            {
                poi = value;
            }
        }

        public AddPointOfInterest()
        {

        }

        public override void Activate()
        {
            base.Activate();
        }
        public override void Deactivate()
        {
            base.Deactivate();
        }
        public override void Update()
        {
            base.Update();
        }
        public override void OnClick()
        {
            base.OnClick();

            float dist = 0.0f;
            Road road = Road.ClosestToCursor(ref dist, CursorPos(Intersection.Z_POSITION));

            if (road != null)
            {
                Intersection i = road.SplitWithDegenerate(CursorPos(Intersection.Z_POSITION));
                i.POI = poi;
            }
        }
    }
}
