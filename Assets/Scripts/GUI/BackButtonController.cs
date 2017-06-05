using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BackButtonController : JMBaseMenuInteractable
{
    private Transform activePauseMenu;
    public override void DoStart()
    {
        activePauseMenu = transform.parent;
        base.DoStart();
    }
    
    protected override void DoPointerDown()
    {
        activePauseMenu.gameObject.SetActive(false);
        LoadingScreenController.Show();
        SceneManager.LoadSceneAsync("MainMenu");
        base.DoPointerDown();
    }
}
