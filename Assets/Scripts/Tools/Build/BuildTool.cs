using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Build
{
    public class BuildTool : Tool
    {
        protected static GameObject cursor;
        public static GameObject Cursor
        {
            get
            {
                return cursor;
            }
            set
            {
                cursor = value;
                cursor.SetActive(false);
            }
        }

        public BuildTool()
        {

        }

        public override void Activate()
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
            cursor.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), Time.deltaTime * 180.0f);
            //position cursor
            cursor.transform.position = CursorPos(CURSOR_Z);

        }
        public override void OnClick()
        {

        }
    }
}
