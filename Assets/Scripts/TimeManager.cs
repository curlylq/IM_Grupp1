using System;
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
}
