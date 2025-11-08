using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class PrefabGameController : MonoBehaviour
{
    public UnityEngine.UI.Text rangeText;
    private int score;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) {
            EventRangeUp(5);
        }
    
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            EventRangeDown(5);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void EventRangeUp( int increment = 1 ) {
        if( score + increment > 100 ) { return; }
        score += increment;
        rangeText.text = score.ToString();
        SteelSeries.GameSense.GSClient.Instance.SendEvent("RANGED-EVENT", score);
    }

    public void EventRangeDown( int increment = 1 ) {
        if( score - increment < 0 ) { return; }
        score -= increment;
        rangeText.text = score.ToString();
        SteelSeries.GameSense.GSClient.Instance.SendEvent("RANGED-EVENT", score);
    }

    public void EventToggle( bool value ) {
        SteelSeries.GameSense.GSClient.Instance.SendEvent("BINARY-EVENT", Convert.ToInt32(value));
    }

    public void SwitchScenes() {
        SceneManager.LoadScene("GSScriptedScene");
    }
}
