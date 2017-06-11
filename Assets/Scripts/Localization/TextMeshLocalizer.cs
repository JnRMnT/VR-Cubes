using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshLocalizer : JMLocalizable
{
    public override void SetText()
    {
        this.GetComponent<TextMesh>().text = LanguageManager.GetString(ResourceKey);
        base.SetText();
    }
}