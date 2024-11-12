using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.pfodmpmiyws9")]
public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private Vector2 _arrowOffset;
    [SerializeField] private Vector2 _handOffset;

    private Camera _canvasCamera;
    private RectTransform _canvas;
    private RectTransform _arrow;
    private RectTransform _hand;

    private void Start() => InitializeCursor();

    private void InitializeCursor()
    {
        // Instantiate cursor UI from resources
        GameObject cursorObj = Instantiate(Resources.Load<GameObject>("Menu/Canvas_Mouse"));

        // Set up the canvas camera for world space rendering
        _canvasCamera = Camera.main;
        cursorObj.GetComponent<Canvas>().worldCamera = _canvasCamera;

        // Cache RectTransforms for arrow and hand cursors
        _canvas = cursorObj.GetComponent<RectTransform>();
        _arrow = cursorObj.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        _hand = cursorObj.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();

        // Hide the system cursor and initialize with default cursor
        Cursor.visible = false;
        ChangeCursorTo(CursorType.Arrow);
    }

    private void Update()
    {
        // Only update cursor if mouse is within screen bounds
        Vector2 screenPoint = Input.mousePosition;
        if (IsMouseOnScreen(screenPoint))
        {
            Vector2 worldPos = ScreenToCanvasPosition(screenPoint);
            UpdateCursorPosition(worldPos);
        }
    }

    private bool IsMouseOnScreen(Vector2 screenPoint)
    {
        return screenPoint.x >= 0 && screenPoint.x <= Screen.width &&
               screenPoint.y >= 0 && screenPoint.y <= Screen.height;
    }

    private void UpdateCursorPosition(Vector2 worldPos)
    {
        // Update positions of arrow and hand cursor relative to screen coordinates
        _arrow.localPosition = worldPos + _arrowOffset;
        _hand.localPosition = worldPos + _handOffset;
    }

    private Vector2 ScreenToCanvasPosition(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas, screenPoint, _canvasCamera, out Vector2 localPoint);
        return localPoint;
    }

    public void ChangeCursorTo(CursorType type)
    {
        // If on mobile, hide both cursors
        if (IsMobile())
        {
            _arrow.gameObject.SetActive(false);
            _hand.gameObject.SetActive(false);
            return;
        }

        // Toggle visibility of the correct cursor based on the selected type
        _arrow?.gameObject?.SetActive(type == CursorType.Arrow);
        _hand?.gameObject?.SetActive(type == CursorType.Hand);
    }

    public void ResetCursor() => ChangeCursorTo(CursorType.Arrow);

    private bool IsMobile()
    {
        return Application.isMobilePlatform;
    }
}
