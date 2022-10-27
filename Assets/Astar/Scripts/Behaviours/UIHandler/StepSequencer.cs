using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Astar
{
    public class StepSequencer : MonoBehaviour
    {
        private AstarAlgorithm algorithm;

        // Start is called before the first frame update
        void Start()
        {
            var button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var mapBehaviour = GameObject.FindObjectOfType<AstarMapBehaviour>();
            Assert.IsNotNull(mapBehaviour);
            mapBehaviour.StepForward(mapBehaviour);
            /*
            if (algorithm == null)
            {
                algorithm = AstarAlgorithm.GetInstance();
                algorithm.Clear();

                var mapBehaviour = GameObject.FindObjectOfType<AstarMapBehaviour>();
                Assert.IsNotNull(mapBehaviour);
                //algorithm.Setup(mapBehaviour.Map);
            }

            Assert.IsNotNull(algorithm);
            algorithm?.StepForward();
            */
        }
    }
}
