using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float limitTime;
    public TextMeshProUGUI timer;
    Timer time;

    // Update is called once per frame
    public void Update()
    {
        time = FindObjectOfType<Timer>();
        timer.gameObject.SetActive(true);

        limitTime -= Time.deltaTime;
        timer.text = $"{Mathf.RoundToInt(limitTime)}";

        if (limitTime <= 0)
        {
            time.enabled = false ;
            timer.text = "";
        }
    }

    public void Restart()
    {
        limitTime = 5;
        Update();
    }
}
