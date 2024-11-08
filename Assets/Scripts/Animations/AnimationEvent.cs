using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Sounds _sound = new Sounds();
    [SerializeField] private float _audioVolume = 0.5f;
    [SerializeField] private float _minPitch = 0.8f;
    [SerializeField] private float _maxPitch = 1.2f;

    [Space(5)]
    [SerializeField] private UnityEvent _animEvent;
   
    public void StartEvent() => _animEvent?.Invoke();

    public void PlaySound()
    {
        if (_sound == Sounds.None)
            return;

        AudioManager.PlayFromResources(_sound, _audioVolume, Random.Range(_minPitch, _maxPitch));
    }
}