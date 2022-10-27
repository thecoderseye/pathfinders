using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astar
{
    public class ClearButton : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClear);
        }

        private void OnClear()
        {
            var mapBehaviour = GameObject.FindObjectOfType<AstarMapBehaviour>();
            mapBehaviour?.Clear();
        }
    }
}
