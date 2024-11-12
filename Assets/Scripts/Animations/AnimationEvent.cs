using UnityEngine;
using UnityEngine.Events;

[HelpURL("https://docs.google.com/document/d/1oEp6sHNLkIlHb_yE7KQcJDd3CRWB1CKEoaNf20HlOek/edit?tab=t.0#heading=h.f7l8f6d2w1nv")]
public class AnimationEvent : MonoBehaviour
{
    [SerializeField] private Sound _sound = new Sound();
    [SerializeField] private float _audioVolume = 0.5f;
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.2f;

    [Space(5)]
    [SerializeField] private UnityEvent _animEvent;
   
    public void StartEvent() => _animEvent?.Invoke();

    public void PlaySound()
    {
        if (_sound == Sound.None)
            return;

        AudioManager.PlayFromResources(_sound, _audioVolume, Random.Range(_minPitch, _maxPitch));
    }
}