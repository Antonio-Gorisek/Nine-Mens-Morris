using UnityEngine;
using System.Collections;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.m7cniar2755y")]
public class PieceAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private float _moveDuration = 1.0f;
    [SerializeField] private float _fadeDuration = 0.5f;

    private SpriteRenderer _spriteRenderer;
    private Vector3 _targetPosition;

    [HideInInspector] public bool isAnimating = false;    

    /// <summary>
    /// Starts the placement animation by setting the target position and initializing the piece.
    /// </summary>
    /// <param name="target">The position where the piece will be placed.</param>
    public void StartPlaceAnimation(Vector3 target)
    {
        _targetPosition = target;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPosition = new Vector3(Random.Range(-10f, 10f), -5, 0);
        transform.position = _startPosition;
        _spriteRenderer.color = new Color(1, 1, 1, 0); // Set the initial alpha to 0 for fade-in effect
        StartCoroutine(AnimatePiece()); // Start the animation coroutine
    }

    /// <summary>
    /// Coroutine that animates the piece's movement and fade-in effect.
    /// </summary>
    private IEnumerator AnimatePiece()
    {
        // Use the longer of the two durations (movement or fade)
        float totalDuration = Mathf.Max(_moveDuration, _fadeDuration);
        float elapsedTime = 0.0f;
        isAnimating = true;

        // Loop until the longer duration is completed
        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime; 

            // Calculate fade and movement values based on elapsed time
            float t = Mathf.Clamp01(elapsedTime / _fadeDuration); // Fade value (0 to 1)
            float moveT = Mathf.Clamp01(elapsedTime / _moveDuration); // Movement value (0 to 1)

            // Apply the fade-in effect to the piece
            _spriteRenderer.color = new Color(1, 1, 1, t);
            // Move the piece towards the target position
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, moveT);

            yield return null;
        }

        // Play the sound effect once the animation is complete
        AudioManager.PlayFromResources(Sound.PiecePlace, 0.5f, 1.1f);

        isAnimating = false;

        // Ensure the piece ends up exactly at the target position and is fully visible
        transform.position = _targetPosition;
        _spriteRenderer.color = new Color(1, 1, 1, 1); 
    }
}