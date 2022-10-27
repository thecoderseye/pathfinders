using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

namespace Astar
{
    public class AstarHeuristicSelector : MonoBehaviour
    {
        private AstarAlgorithm.HeuristicMethod activeHeuristic;
        public AstarAlgorithm.HeuristicMethod GetSelectedHeuristic() => activeHeuristic;

        public Toggle Manhattan;
        public Toggle Euclidean;
        public Toggle Hybrid;

        private ToggleGroup toggleGroup;

        // Start is called before the first frame update
        void Start()
        {
            toggleGroup = gameObject.GetComponent<ToggleGroup>();

            Manhattan.onValueChanged.AddListener(OnSelected);
            Euclidean.onValueChanged.AddListener(OnSelected);
            Hybrid.onValueChanged.AddListener(OnSelected);

            var active = toggleGroup.ActiveToggles().First();
            toggleGroup.NotifyToggleOn(active);
        }

        private void OnSelected(bool toggle)//AstarAlgorithm.HeuristicMethod heuristic)
        {
            if (toggle)
            {
                var activeToggle = toggleGroup.ActiveToggles().First();
                activeHeuristic = Enum.Parse<AstarAlgorithm.HeuristicMethod>(activeToggle.name);
            }
        }
    }
}
