using System.Collections;
using UnityEngine;

/// <summary>
/// Anime un SpriteRenderer en changeant de sprite à intervalle régulier.
/// Simple alternative à l'Animator pour des animations en boucle.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    [Header("Sprites")]
    [Tooltip("Les sprites de l'animation, dans l'ordre.")]
    public Sprite[] frames;

    [Header("Vitesse")]
    [Tooltip("Temps en secondes entre chaque frame.")]
    public float frameInterval = 0.1f;

    [Header("Lecture")]
    [Tooltip("Si activé, chaque frame est choisie aléatoirement parmi les sprites.\n" +
             "Si désactivé, les sprites sont lus dans l'ordre.")]
    public bool randomOrder = false;

    [Header("Délai de démarrage")]
    [Tooltip("Si activé, l'animation attend un délai aléatoire avant de démarrer.")]
    public bool randomStartDelay = false;

    [Tooltip("Délai minimum en secondes avant le démarrage de l'animation.")]
    public float minDelay = 0f;

    [Tooltip("Délai maximum en secondes avant le démarrage de l'animation.")]
    public float maxDelay = 2f;

    // ── Runtime ────────────────────────────────────────────────────────────────

    private SpriteRenderer _spriteRenderer;
    private int _currentFrame = 0;
    private float _timer = 0f;
    private bool _playing = false;

    // ── Unity ──────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (randomStartDelay)
            StartCoroutine(StartAfterDelay());
        else
            _playing = true;
    }

    private void Update()
    {
        if (!_playing) return;
        if (frames == null || frames.Length == 0) return;

        _timer += Time.deltaTime;

        if (_timer >= frameInterval)
        {
            _timer -= frameInterval;
            _currentFrame = randomOrder ? PickRandom() : (_currentFrame + 1) % frames.Length;
            _spriteRenderer.sprite = frames[_currentFrame];
        }
    }

    // ── Coroutine ──────────────────────────────────────────────────────────────

    private IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        _playing = true;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private int PickRandom()
    {
        if (frames.Length == 1) return 0;

        int next;
        do { next = Random.Range(0, frames.Length); }
        while (next == _currentFrame);

        return next;
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>Remet l'animation à la première frame.</summary>
    public void ResetAnimation()
    {
        _playing = false;
        _currentFrame = 0;
        _timer = 0f;

        if (frames != null && frames.Length > 0)
            _spriteRenderer.sprite = frames[0];

        if (randomStartDelay)
            StartCoroutine(StartAfterDelay());
        else
            _playing = true;
    }
}