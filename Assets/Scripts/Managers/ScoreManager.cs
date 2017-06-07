using System;
using UnityEngine;

public class ScoreManager : JMBehaviour
{

    public static int HighScore
    {
        get
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }
        set
        {
            PlayerPrefs.SetInt("HighScore", value);
        }
    }

    private TextMesh scoreText;
    public static int Score
    {
        get
        {
            return Mathf.RoundToInt(timeScore) + bonusScore;
        }
    }
    protected static int bonusScore;
    protected static float timeScore;

    public override void DoStart()
    {
        scoreText = GetComponent<TextMesh>();
        ResetScore();
        base.DoStart();
    }

    public static void ResetScore()
    {
        bonusScore = 0;
        timeScore = 0;
    }

    public static void HandleChain(int chainLength)
    {
        bonusScore += (chainLength - 2) * 10;
    }

    public override void DoFixedUpdate()
    {
        timeScore += Time.fixedDeltaTime;
        base.DoFixedUpdate();
    }

    public override void DoUpdate()
    {
        if (scoreText != null)
        {
            scoreText.text = LanguageManager.GetString("SCORE") + ": " + Score.ToString();
        }
        base.DoUpdate();
    }
}
