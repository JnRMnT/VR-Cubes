using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenInnerController : JMBaseMenuInteractable
{
    protected override void DoPointerDown()
    {
        LoadingScreenController.Show();
        if (this.name == "ReplayButton")
        {
            SceneManager.LoadSceneAsync("MainScene");
        }
        else if (this.name == "MainMenuButton")
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
        base.DoPointerDown();
    }
}