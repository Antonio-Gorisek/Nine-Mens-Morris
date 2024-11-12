using System.Collections;
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.tlgl95g593of")]
public class AnimationPopup : MonoBehaviour
{
    [SerializeField] private Sound _sounds;
    [SerializeField] private AnimationCurve _popupCurve;
    [SerializeField] private AnimationCurve _hideCurve;
    [Space(10)]
    [SerializeField] private float _targetScale = 1.0f;
    [SerializeField] private float _speed = 2.0f;
    private bool _isAnimating = false;

    public void ShowPopup() => StartCoroutine(PopupAnim(true));

    public void HidePopup(bool destroyParent = false) => StartCoroutine(PopupAnim(false, destroyParent));

    /// <summary>
    /// Coroutine to handle both showing and hiding animations
    /// </summary>
    private IEnumerator PopupAnim(bool isShowing, bool destroyParent = false)
    {
        if (_isAnimating)
            yield return null;

        _isAnimating = true;
        float timeElapsed = 0f;

        // Select the appropriate curve based on whether showing or hiding
        AnimationCurve curve = isShowing ? _popupCurve : _hideCurve;
        AudioManager.PlayFromResources(_sounds, 0.5f);
        while (timeElapsed < 1f)
        {
            // Calculate scale based on the animation curve
            float scaleAmount = curve.Evaluate(timeElapsed) * _targetScale;
            // Apply scale to transform
            transform.localScale = new Vector2(scaleAmount, scaleAmount);
            timeElapsed += Time.deltaTime * _speed;
            yield return null;
        }

        // Set final scale after animation completes
        transform.localScale = isShowing ? new Vector2(_targetScale, _targetScale) : Vector2.zero;
        _isAnimating = false;

        if (isShowing == false)
            Destroy(destroyParent ? gameObject.transform.parent.gameObject : gameObject);
    }
}
