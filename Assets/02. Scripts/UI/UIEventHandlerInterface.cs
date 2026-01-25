using UnityEngine;
using UnityEngine.EventSystems;

public interface UIEventHandler : IPointerDownHandler, IDragHandler, IEndDragHandler
{
    void OnPointerDown(PointerEventData eventData);
    void OnDrag(PointerEventData eventData);
    void OnEndDrag(PointerEventData eventData);
}