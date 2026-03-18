using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [Header("Game Timer")]
    [SerializeField] private float endTime = 90f;
    private float currentTime = 0f;
    private bool isRunning = false;

    public Action<float> OnTimeUpdated;
    public Action OnTimeUp;

    [Header("Countdown")]
    [SerializeField] private float countdownStart = 3f;
    public TMP_Text countdownText;

    [Header("References")]
    public SimpleSpawner simpleSpawner;
    public UIManager uiManager;

    private bool hasStarted = false;

    private void Update()
    {
        if (!isRunning) return;

        currentTime += Time.deltaTime;
        OnTimeUpdated?.Invoke(currentTime);

        if (currentTime >= endTime)
        {
            isRunning = false;
            OnTimeUp?.Invoke();

            if (simpleSpawner != null)
                simpleSpawner.StopSpawning();

            if (uiManager != null)
                uiManager.ShowGameOver();
        }
    }

    public void StartCountdownAndGame()
    {
        if (hasStarted) return;
        hasStarted = true;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        float timeLeft = countdownStart;

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        while (timeLeft > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(timeLeft).ToString();

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        if (countdownText != null)
        {
            countdownText.text = "KÖR!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        currentTime = 0f;
        isRunning = true;

        if (uiManager != null)
            uiManager.ShowGameUI();

        if (simpleSpawner != null)
            simpleSpawner.StartSpawning();
    }
}

/*using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TimeManager : MonoBehaviour
{

    [SerializeField] private float startTime = 0f;
    [SerializeField] private float endTime = 90f;
    private bool isRunning = true;

    public Action<float> OnTimeUpdated;
    public Action OnTimeUp;

    [SerializeField] private float timeLeft = 3f;
    public TMP_Text countdownText;

    public SimpleSpawner SimpleSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning) return;

        startTime += Time.deltaTime;

        OnTimeUpdated?.Invoke(startTime);

        if (startTime >= endTime)
        {
            isRunning = false;
            OnTimeUp?.Invoke();
        }
    }

   public void countDown()
    {
        timeLeft -= Time.deltaTime;
        countdownText.text = (timeLeft).ToString("0");
        if (startTime < 0)
        {
            SimpleSpawner.StartSpawning();
        }
    }
}*/
