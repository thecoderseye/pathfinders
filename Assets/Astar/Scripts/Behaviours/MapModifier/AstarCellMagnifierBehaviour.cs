using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Astar
{
    public class AstarCellMagnifierBehaviour : MonoBehaviour, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private event Action<AstarNode, bool> onMagnifying;
        public event Action<AstarNode, bool> OnMagnifying
        {
            add { onMagnifying += value; }
            remove { onMagnifying -= value; }
        }

        void IPointerMoveHandler.OnPointerMove(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            switch (self.Node.Status.Value)
            {
                case NodeStatusLiteral.Block:
                case NodeStatusLiteral.None:
                    break;

                default:
                    if (eventData.pointerCurrentRaycast.isValid)
                        onMagnifying?.Invoke(self.Node, true);

                    break;

            }
        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            switch (self.Node.Status.Value)
            {
                case NodeStatusLiteral.Block:
                case NodeStatusLiteral.None:
                    break;

                default:
                    onMagnifying?.Invoke(self.Node, false);
                    break;
            }
        }

        private AstarCellRenderer self;
        private AstarMapBehaviour mapBehaviour;

        // Start is called before the first frame update
        void Start()
        {
            self = gameObject.GetComponent<AstarCellRenderer>();
            mapBehaviour = GameObject.FindObjectOfType<AstarMapBehaviour>();
        }
    }
}
