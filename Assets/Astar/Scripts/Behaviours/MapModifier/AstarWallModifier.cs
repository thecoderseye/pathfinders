using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Astar
{
    public class AstarWallModifier : IAstarMapModifier
    {
        private readonly AstarMapBehaviour mapBehaviour;
        private AstarNode nodeOnHovering;

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            ToggleBlockNode(eventData);
        }

        private void ToggleBlockNode(PointerEventData eventData)
        {
            var cellRenderer = eventData.pointerCurrentRaycast.gameObject.GetComponent<AstarCellRenderer>();
            if (cellRenderer == null)
                return;

            var node = cellRenderer.Node;

            if (nodeOnHovering == node)
                return;

            nodeOnHovering = node;

            switch (nodeOnHovering.Status.Value)
            {
                case NodeStatusLiteral.Block:
                    nodeOnHovering.SetNodeStatus(new NodeStatus(NodeStatusLiteral.None));
                    nodeOnHovering.SetNodeType(NodeType.Walkable);
                    break;

                case NodeStatusLiteral.None:
                    nodeOnHovering.SetNodeStatus(new NodeStatus(NodeStatusLiteral.Block));
                    nodeOnHovering.SetNodeType(NodeType.Wall);
                    break;
            }
        }

        void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            ToggleBlockNode(eventData);
        }

        void IPointerDownHandler.OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IPointerUpHandler.OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        { }

        void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IEndDragHandler.OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        void IDropHandler.OnDrop(UnityEngine.EventSystems.PointerEventData eventData)
        { }

        public AstarWallModifier(AstarMapBehaviour mapBehaviour)
        {
            this.mapBehaviour = mapBehaviour;
        }
    }
}
