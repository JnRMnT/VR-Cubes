using UnityEngine;

public class LoadingScreenController : JMBehaviour
{
    public static LoadingScreenController Instance;
    public override void DoStart()
    {
        Instance = this;
        Hide();
        base.DoStart();
    }

    public static void Show()
   {
        Time.timeScale = 0;
        Instance.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        Time.timeScale = 1;
        Instance.gameObject.SetActive(false);
    }

    public override void DoUpdate()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward * 20f;
        transform.LookAt(Camera.main.transform);
        base.DoUpdate();
    }
}
