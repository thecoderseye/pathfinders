using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Astar
{
    public interface IAstarMapModifier : IPointerExitHandler, IPointerEnterHandler, IDropHandler, IInitializePotentialDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler
    { }
}
