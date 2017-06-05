using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextLocalizer : JMBehaviour
{
    public bool IsLocalizable = true;
    public string ResourceKey;

    public override void DoStart()
    {
        if (IsLocalizable)
        {
            this.GetComponent<Text>().text = LanguageManager.GetString(ResourceKey);
        }

        base.DoStart();
    }
}
