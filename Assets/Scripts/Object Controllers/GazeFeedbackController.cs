using UnityEngine;

public class GazeFeedbackController: JMBehaviour
{
    public override void DoStart()
    {
        gameObject.SetActive(false);
        base.DoStart();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}