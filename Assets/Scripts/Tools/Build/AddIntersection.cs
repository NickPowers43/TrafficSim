using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Build
{
    class AddIntersection : BuildTool
    {
        public AddIntersection()
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

            //color cursor appropriately
            if (!Intersection.CheckOverlap(CursorPos(Intersection.Z_POSITION), Intersection.INITIAL_RADIUS))
            {
                //highlight cursor green
                cursor.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f);
            }
            else
            {
                //highlight cursor red
                cursor.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f);
            }
        }
        public override void OnClick()
        {
            base.OnClick();

            if (!Intersection.CheckOverlap(CursorPos(Intersection.Z_POSITION), Intersection.INITIAL_RADIUS))
            {
                GameObject nodeGO = (GameObject)GameObject.Instantiate(intersectionPrefab);
                nodeGO.transform.position = CursorPos(Intersection.Z_POSITION);
            }
        }
    }
}
