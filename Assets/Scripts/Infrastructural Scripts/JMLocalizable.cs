using UnityEngine;

public class JMLocalizable : JMBehaviour
{
    public bool IsLocalizable = true;
    public string ResourceKey;
    public override void DoStart()
    {
        if (IsLocalizable)
        {
            SetText();
        }
        
        base.DoStart();
    }
    
    public virtual void SetText()
    {
        //This method will be overriden to update UI
    }

    public void UpdateText()
    {
        if (IsLocalizable)
        {
            SetText();
        }
    }
}