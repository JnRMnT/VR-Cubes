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

        MainPlayerController.Instance.transform.position = new Vector3(MainPlayerController.Instance.transform.position.x, MainPlayerController.Instance.transform.position.y + Cube.Size.y * (GameManager.GridSize.y + 3), MainPlayerController.Instance.transform.position.z);
        MainPlayerController.Instance.transform.rotation = Quaternion.identity;
        MainPlayerController.ChangeRotation(90, 0, 0);
        Vector3 position = MainPlayerController.Instance.transform.position + Vector3.down * Cube.Size.y;
        RaycastHit blockingObject;
        if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out blockingObject, Cube.Size.y))
        {
            position = Vector3.MoveTowards(blockingObject.point, Camera.main.transform.position, 2f);
        }
        Instance.transform.position = position;
        Instance.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}