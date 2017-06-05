using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeButtonController : JMBaseMenuInteractable
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
        GameManager.ResumeGame();
        base.DoPointerDown();
    }
}
