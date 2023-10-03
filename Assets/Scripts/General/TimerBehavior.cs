using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TimerBehavior : MonoBehaviour
{
    //For Timer
    static DateTime initTime;
    //Timer
    [SerializeField] public static TextMeshProUGUI TimerText;
    private void Start()
    {
        TimerText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<TextMeshProUGUI>();
    }
    public static void StartTimer()
    {
        initTime = DateTime.Now;
        TimerText.gameObject.SetActive(false);
    }
    public static void EndTimer()
    {
        TimerText.gameObject.SetActive(true);
        TimerText.text = (DateTime.Now - initTime).ToString();
    }

}
