using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreenController : JMBehaviour
{
    public TextMesh HUDScore;
    public Image StarImage;
    public Text ScoreText, HighScoreText;
    public static GameOverScreenController Instance;

    public override void DoStart()
    {
        Instance = this;
        Instance.gameObject.SetActive(false);
        base.DoStart();
    }

    public static void Show()
    {
        Instance.HUDScore.gameObject.SetActive(false);
        Instance.ScoreText.text = ScoreManager.Score.ToString();
        if (ScoreManager.Score > ScoreManager.HighScore)
        {
            ScoreManager.HighScore = ScoreManager.Score;
            Instance.StarImage.gameObject.SetActive(true);
        }
        else
        {
            Instance.StarImage.gameObject.SetActive(false);
        }
        Instance.HighScoreText.text = ScoreManager.HighScore.ToString();

        Instance.gameObject.SetActive(true);
        Instance.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 30f;
        Instance.transform.position = new Vector3(Instance.transform.position.x, 60, Instance.transform.position.z);
        Instance.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z);
    }
}