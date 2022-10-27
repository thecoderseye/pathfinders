using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Astar
{
    public class StartButton : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnStart);
        }

        private void OnStart()
        {
            var mapBehaviour = GameObject.FindObjectOfType<AstarMapBehaviour>();
            Assert.IsNotNull(mapBehaviour);
            mapBehaviour.StartCoroutine(mapBehaviour.StartFindingPath(mapBehaviour));
        }
    }
}
