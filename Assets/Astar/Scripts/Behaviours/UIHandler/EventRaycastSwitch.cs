using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Astar
{
    public class EventRaycastSwitch : IDisposable
    {
        void IDisposable.Dispose()
        {
            raycaster.enabled = toggle ? false : true;
        }

        private readonly GraphicRaycaster raycaster;
        private readonly bool toggle;

        public EventRaycastSwitch(ref GraphicRaycaster raycaster, bool toggle = false)
        {
            this.raycaster = raycaster;
            this.raycaster.enabled = toggle;
        }
    }
}
