using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextLocalizer : JMLocalizable
{
    public override void SetText()
    {
        this.GetComponent<Text>().text = LanguageManager.GetString(ResourceKey);
        base.SetText();
    }
}
