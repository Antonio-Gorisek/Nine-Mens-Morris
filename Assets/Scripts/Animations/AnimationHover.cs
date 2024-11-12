using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.1j0uag610t5")]
public class AnimationHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sound _hoverSound = Sound.None;
    [SerializeField] private float _zoomFactor = 1.2f;
    [SerializeField] private float _zoomDuration = 0.2f;
    [SerializeField] private float _rotationAngle = 2f;

    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private bool _isZoomed = false;

    void Start()
    {
        // Store the initial scale and rotation of the object
        _originalScale = transform.localScale;
        _originalRotation = transform.rotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play hover sound with randomized pitch
        AudioManager.PlayFromResources(_hoverSound, 0.2f, Random.Range(1.5f, 1.7f));
        // Toggle zoom and rotation effect
        ToggleZoom();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Toggle zoom and rotation effect
        ToggleZoom();
    }

    private void ToggleZoom()
    {
        // Stop any ongoing animations
        StopAllCoroutines();

        // Determine target scale and rotation based on current state
        Vector3 targetScale = _isZoomed ? _originalScale : _originalScale * _zoomFactor;
        Quaternion targetRotation = _isZoomed ? _originalRotation : Quaternion.Euler(0, 0, _rotationAngle);

        StartCoroutine(ZoomAndRotate(targetScale, targetRotation));

        _isZoomed = !_isZoomed;
    }

    private IEnumerator ZoomAndRotate(Vector3 targetScale, Quaternion targetRotation)
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.rotation;

        // Lerp between the current and target scale and rotation
        while (elapsedTime < _zoomDuration)
        {
            // Smoothly interpolate scale and rotation
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / _zoomDuration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / _zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set
        transform.localScale = targetScale;
        transform.rotation = targetRotation;
    }
}
