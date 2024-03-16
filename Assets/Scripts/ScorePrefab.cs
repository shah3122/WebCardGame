using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScorePrefab : MonoBehaviour
{
    private string playerName;
    private int score;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;


    public void SetUpScorePrefab(string pName, int pScore)
    {
        playerName = pName;
        score = pScore;
        UpdateUI();
    }

    private void UpdateUI()
    {
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }
}
