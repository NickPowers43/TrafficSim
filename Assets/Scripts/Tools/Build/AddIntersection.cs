using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Build
{
    class AddIntersection : Tool
    {
        private GameObject intersectionPrefab;
        private GameObject cursor;

        public AddIntersection(GameObject intersectionPrefab, GameObject cursor)
        {
            this.intersectionPrefab = intersectionPrefab;
            this.cursor = cursor;
        }

        public void Activate()
        {
            cursor.SetActive(true);
        }
        public override void Deactivate()
        {
            cursor.SetActive(false);
        }
        public override void Update()
        {
            //rotate cursor
            cursor.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * 360.0f);
            //position cursor
            cursor.transform.position = CursorPos(CURSOR_Z);

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
            GameObject nodeGO = (GameObject)GameObject.Instantiate(intersectionPrefab);
            nodeGO.transform.position = CursorPos(Intersection.Z_POSITION);

            //UpdateRoadNetwork();
        }
    }
}
