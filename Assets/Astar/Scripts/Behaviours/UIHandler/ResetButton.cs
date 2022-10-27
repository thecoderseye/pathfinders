using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astar
{
    public class ResetButton : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnReset);
        }

        private void OnReset()
        {
            var mapBehaviour = GameObject.FindObjectOfType<AstarMapBehaviour>();
            mapBehaviour?.Reset();
        }
    }
}
