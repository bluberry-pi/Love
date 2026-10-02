using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TMP_Text timerText;

    public float time = 5f;

    void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0)
        {
            time = 5f;
        }

        timerText.text = Mathf.CeilToInt(time).ToString();
    }
}