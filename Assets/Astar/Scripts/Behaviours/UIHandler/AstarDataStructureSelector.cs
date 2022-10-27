using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Astar
{
    public class AstarDataStructureSelector : MonoBehaviour
    {
        public enum Type
        {
            FIFO,
            LIFO,
        }

        public Type GetSelectedType() => dataStructureType;

        private Type dataStructureType;
        private ToggleGroup group;

        
        public Toggle FIFO;
        public Toggle LIFO;

        // Start is called before the first frame update
        void Start()
        {
            group = gameObject.GetComponent<ToggleGroup>();

            FIFO.onValueChanged.AddListener(OnFifoSelected);
            LIFO.onValueChanged.AddListener(OnLifoSelected);

            var active = group.ActiveToggles().First();
            group.NotifyToggleOn(active);
        }

        private void OnFifoSelected(bool toggle)
        {
            dataStructureType = Type.FIFO;
        }

        private void OnLifoSelected(bool toggle)
        {
            dataStructureType = Type.LIFO;
        }
    }
}
