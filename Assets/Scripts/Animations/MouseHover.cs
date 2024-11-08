using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

public class MouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent OnMouseEnterEvent;
    public UnityEvent OnMouseExitEvent;
    public UnityEvent OnMouseClickEvent;

    public void OnPointerClick(PointerEventData eventData) => OnPointerClick();
    public void OnPointerEnter(PointerEventData eventData) => OnHoverEnter();
    public void OnPointerExit(PointerEventData eventData) => OnHoverExit();

    private void OnMouseEnter() => OnHoverEnter();

    private void OnMouseExit() => OnHoverExit();

    private void OnMouseDown() => OnPointerClick();

    private void OnPointerClick()
    {
        OnMouseClickEvent.Invoke();
        CursorManager.Instance.ChangeCursorTo(CursorType.Arrow);
    }

    private void OnHoverEnter()
    {
        OnMouseEnterEvent.Invoke();
        CursorManager.Instance.ChangeCursorTo(CursorType.Hand);
    }

    private void OnHoverExit()
    {
        OnMouseExitEvent.Invoke();
        CursorManager.Instance.ChangeCursorTo(CursorType.Arrow);
    }
}
