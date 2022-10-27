using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astar
{
    public class AstarDirectionSelector : MonoBehaviour
    {
        private Toggle toggleIncardinal;

        public AstarAlgorithm.Directions GetDirection() => toggleIncardinal.isOn ? AstarAlgorithm.Directions.Intercardinal : AstarAlgorithm.Directions.CardinalOnly;

        // Start is called before the first frame update
        void Start()
        {
            toggleIncardinal = gameObject.GetComponent<Toggle>();
        }
    }
}
