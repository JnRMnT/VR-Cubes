using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : JMBaseMenuInteractable
{
    private Transform settingsMenu;
    public override void DoStart()
    {
        if (this.name == "QuitButton" && Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameObject.SetActive(false);
        }
        if (this.name == "GazeToClickToggle")
        {
            settingsMenu = transform.parent.parent;
            var toggle = GetComponent<Toggle>();
            if (!ConfigurationManager.IsGazeClickEnabled)
            {
                ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            }
        }
        else if (this.name == "MusicToggle")
        {
            settingsMenu = transform.parent.parent;
            var toggle = GetComponent<Toggle>();
            if (!ConfigurationManager.BackgroundMusicOn)
            {
                ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            }
        }
        base.DoStart();
    }

    protected override void DoPointerDown()
    {
        if (this.name == "StartGameButton")
        {
            LoadingScreenController.Show();
            SceneManager.LoadSceneAsync("MainScene");
        }
        else if (this.name == "SettingsButton")
        {
            MainPlayerController.Instance.transform.rotation = Quaternion.identity;
            MainPlayerController.ChangeRotation(0, 75, 0);
        }
        else if (this.name == "QuitButton")
        {
            Application.Quit();
        }
        else if (this.name == "GazeToClickToggle" || this.name == "MusicToggle")
        {
            var toggle = GetComponent<Toggle>();
            if (internalClick)
            {
                ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            }
        }
        else if (this.name == "LanguageSelector")
        {
            LanguageManager.UpdateLanguage();
        }
        base.DoPointerDown();
    }

    public void GazeToClickToggleChanged(bool isChecked)
    {
        ConfigurationManager.IsGazeClickEnabled = isChecked;
    }

    public void MusicToggleChanged(bool isOn)
    {
        ConfigurationManager.BackgroundMusicOn = isOn;
    }
}