using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Astar
{
    public class AstarMapModifier : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IDropHandler, IEndDragHandler, IInitializePotentialDragHandler
    {
        private Dictionary<NodeStatusLiteral, IAstarMapModifier> eventHandlers;

        private AstarMapBehaviour mapBehaviour;
        private IAstarMapModifier wallModifier;
        private IAstarMapModifier routeModifier;

        private IAstarMapModifier activeModifier;

        private AstarNode picked;
        private AstarNode droped;
        private AstarNode hovering;

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.dragging)
            {
                activeModifier?.OnPointerEnter(eventData);
            }
        }

        void IDropHandler.OnDrop(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.dragging)
            {
                activeModifier?.OnDrop(eventData);
            }

            activeModifier = null;
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            var gameObject = eventData.pointerCurrentRaycast.gameObject;
            if (gameObject != null)
            {
                var cellRenderer = gameObject.GetComponent<AstarCellRenderer>();
                if (cellRenderer == null)
                    return;

                var node = cellRenderer.Node;
                switch (node.Status.Value)
                {
                    case NodeStatusLiteral.Start:
                    case NodeStatusLiteral.End:
                        activeModifier = routeModifier;
                        break;

                    default:
                        activeModifier = wallModifier;
                        break;
                }

                activeModifier?.OnInitializePotentialDrag(eventData);
            }
        }

        void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (eventData.dragging && eventData.pointerCurrentRaycast.isValid)
            {
                activeModifier?.OnDrag(eventData);
            }
        }

        void IPointerDownHandler.OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IPointerUpHandler.OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        { }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IEndDragHandler.OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        { }


        // Start is called before the first frame update
        void Start()
        {
            mapBehaviour = gameObject.GetComponent<AstarMapBehaviour>();
            routeModifier = new AstarRouteModifier(mapBehaviour);
            wallModifier = new AstarWallModifier(mapBehaviour);
        }
    }
}
