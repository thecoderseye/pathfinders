using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Astar
{
    public class AstarRouteModifier : IAstarMapModifier
    {
        private readonly AstarMapBehaviour mapBehaviour;

        private AstarNode picked;
        private AstarNode droped;
        private AstarNode hovering;

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        void IDropHandler.OnDrop(UnityEngine.EventSystems.PointerEventData eventData)
        {

            if (hovering != null)
            {
                var map = mapBehaviour.Map;
                switch (picked.Status.Value)
                {
                    case NodeStatusLiteral.End:
                        map.SetEndPosition(hovering.Position.x, hovering.Position.y);
                        break;

                    case NodeStatusLiteral.Start:
                        map.SetStartPosition(hovering.Position.x, hovering.Position.y);
                        break;

                }

                picked = null;
                hovering = null;
            }
        }

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            var cellRenderer = eventData.pointerCurrentRaycast.gameObject.GetComponent<AstarCellRenderer>();
            var node = cellRenderer.Node;
            switch (node.Status.Value)
            {
                case NodeStatusLiteral.Start:
                case NodeStatusLiteral.End:
                    picked = new AstarNode(ref node);
                    hovering = node;
                    break;
            }
        }

        void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (picked != null)
            {
                var next = eventData.pointerCurrentRaycast.gameObject.GetComponent<AstarCellRenderer>().Node;
                if (next == hovering)
                    return;

                var ignorable = (picked.Status.Value.Equals(NodeStatusLiteral.Start) && next.Status.Value.Equals(NodeStatusLiteral.End)) ||
                                (picked.Status.Value.Equals(NodeStatusLiteral.End) && next.Status.Value.Equals(NodeStatusLiteral.Start)) ||
                                (next.Status.Value.Equals(NodeStatusLiteral.Block));
                if (ignorable)
                {
                    return;
                }
                else
                {
                    if(hovering != null)
                    {
                        hovering.Reset(NodeStatusLiteral.None);
                        hovering.SetNodeType(NodeType.Walkable);
                        hovering.UpdateScores(0, 0);
                    }

                    hovering = next;
                    hovering.Reset(picked.Status.Value);
                    hovering.SetNodeType(picked.Type);

                    var map = mapBehaviour.Map;
                    switch (picked.Status.Value)
                    {
                        case NodeStatusLiteral.End:
                            map.SetEndPosition(hovering.Position.x, hovering.Position.y);
                            break;

                        case NodeStatusLiteral.Start:
                            map.SetStartPosition(hovering.Position.x, hovering.Position.y);
                            break;
                    }
                }
            }
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

        public AstarRouteModifier(AstarMapBehaviour mapBehaviour)
        {
            this.mapBehaviour = mapBehaviour;
        }
    }
}
