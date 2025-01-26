using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerText : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnTimerEnd;

    [SerializeField]
    private float _startTimeSeconds = 60.0f;

    [SerializeField]
    private string _timerPrefix = "TIME";


    private TextMeshProUGUI _textMesh;
    private float _backingTimerSeconds = 0.0f;
    private bool _didTimerEnd = false;

    void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _backingTimerSeconds = _startTimeSeconds;
    }

    void Start() {
        ResetTimer();
    }

    void Update()
    {
        if (_didTimerEnd)
        {
            OnTimerEnd?.Invoke();
            gameObject.SetActive(false);
            return;
        }

        _backingTimerSeconds -= Time.deltaTime;
        if (_backingTimerSeconds <= 0)
        {
            _textMesh.text = _timerPrefix + "\n 0:00";
            _didTimerEnd = true;
            return;
        }

        int minutes = (int)_backingTimerSeconds / 60;
        int seconds = (int)_backingTimerSeconds % 60;
        _textMesh.text = _timerPrefix + "\n" + minutes + ":" + seconds.ToString("00");
    }

    public void ResetTimer()
    {
        gameObject.SetActive(true);
        _backingTimerSeconds = _startTimeSeconds;
        _didTimerEnd = false;
    }
}
