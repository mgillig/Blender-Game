using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    private bool timerActivated = false;
    private float elapsedTime = 0f;

    // Update is called once per frame
    void Update()
    {
        if (timerActivated)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = string.Format("{00:00}:{01:00}", Mathf.FloorToInt(elapsedTime / 60), Mathf.FloorToInt(elapsedTime % 60));
        }
    }

    public void SetTimerActive(bool active)
    {
        timerActivated = active;
    }

    public void SetFinalTime()
    {
        timerActivated = false;
        timerText.text = string.Format("{00:00}:{01:00}", Mathf.FloorToInt(elapsedTime / 60), Mathf.FloorToInt(elapsedTime % 60));
    }

    public void ResetTime()
    {
        timerActivated = false;
        elapsedTime = 0f;
    }
}
