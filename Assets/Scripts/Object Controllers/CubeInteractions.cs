using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CubeInteractions : JMBehaviour
{
    public TextMesh[] FeedbackMeshes;
    public TextMesh[] ClimbFeedbackMeshes;
    private Cube cube;
    private bool updateFeedback;

    /// <summary>
    /// How much offset is within the limit
    /// </summary>
    protected float pushRequiredOffset = 5f;

    public override void DoStart()
    {
        foreach (TextMesh mesh in FeedbackMeshes)
        {
            mesh.gameObject.SetActive(false);
        }
        foreach (TextMesh mesh in ClimbFeedbackMeshes)
        {
            mesh.gameObject.SetActive(false);
        }
        this.cube = GetComponent<Cube>();
        updateFeedback = false;
        base.DoStart();
    }

    public void OnPointerDown()
    {
        updateFeedback = false;
        if (!MainPlayerController.Instance.IsBusy())
        {
            if (Mathf.Abs(MainPlayerController.Instance.transform.position.y - transform.position.y) < pushRequiredOffset && cube.IsCloseEnoughToPush())
            {
                if (ClimbOrPush())
                {
                    //Do Climb
                    if (cube.CanBeClimbed())
                    {
                        MainPlayerController.Instance.Climb(cube);
                    }
                }
                else
                {
                    //Do Push
                    if (cube.CanBePushed())
                    {
                        cube.Push();
                    }
                }
            }
            else if (MainPlayerController.CanWalkTowards(cube))
            {
                //Do Walk
                MainPlayerController.Instance.WalkTowards(cube);
            }
        }

        if (GetComponent<GazeToClickInteraction>().IsGazing)
        {
            ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        }
    }

    protected bool ClimbOrPush()
    {
        RaycastHit rayHit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out rayHit) && rayHit.point.y >= cube.transform.position.y + (Cube.Size.y / 4))
        {
            //Climb
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnPointerEnter()
    {
        if (GameManager.GameState == GameState.Started)
        {
            if (!MainPlayerController.Instance.IsBusy())
            {
                updateFeedback = true;
                StartCoroutine(UpdateFeedback());
            }
            else
            {
                StartCoroutine(WaitForAction());
            }
        }
    }

    private IEnumerator WaitForAction(int tryCount = 0)
    {
        yield return new WaitForSeconds(0.8f);
        var gazeToClickInteractionScript = GetComponent<GazeToClickInteraction>();
        if (gazeToClickInteractionScript.IsGazing)
        {
            if (!MainPlayerController.Instance.IsBusy())
            {
                OnPointerEnter();
            }
            else if (tryCount < 3)
            {
                gazeToClickInteractionScript.ResetGazeCounter();
                StartCoroutine(WaitForAction(tryCount + 1));
            }
        }
    }

    private IEnumerator UpdateFeedback(int tryCount = 0)
    {
        if (updateFeedback)
        {
            if (Mathf.Abs(MainPlayerController.Instance.transform.position.y - transform.position.y) < pushRequiredOffset && cube.IsCloseEnoughToPush())
            {
                DisableFeedbackTexts();
                if (ClimbOrPush())
                {
                    //Climb
                    if (cube.CanBeClimbed())
                    {
                        ActivateClimb();
                    }
                }
                else
                {
                    //Push
                    if (cube.CanBePushed())
                    {
                        ActivatePush();
                    }
                }
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(UpdateFeedback());
            }
            else if (MainPlayerController.CanWalkTowards(cube))
            {
                //Walk
                ActivateWalk();
            }
            else
            {
                updateFeedback = false;
            }
        }
        else
        {
            if (tryCount < 3)
            {
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(UpdateFeedback(tryCount + 1));
            }
            else
            {
                OnPointerExit();
            }
        }
    }

    protected void ActivateWalk()
    {
        TextMesh feedbackMesh = GetBestTextMesh(FeedbackMeshes, true);
        if (feedbackMesh != null)
        {
            if (ConfigurationManager.IsGazeClickEnabled)
            {
                feedbackMesh.text = LanguageManager.GetString("GazeToWalk");
            }
            else
            {
                feedbackMesh.text = LanguageManager.GetString("ClickToWalk");
            }
        }
    }

    protected void ActivatePush()
    {
        TextMesh feedbackMesh = GetBestTextMesh(FeedbackMeshes, false);
        if (feedbackMesh != null)
        {
            if (ConfigurationManager.IsGazeClickEnabled)
            {
                feedbackMesh.text = LanguageManager.GetString("GazeToPush");
            }
            else
            {
                feedbackMesh.text = LanguageManager.GetString("ClickToPush");
            }
        }
    }

    protected void ActivateClimb()
    {
        TextMesh feedbackMesh = GetBestTextMesh(ClimbFeedbackMeshes, false);
        if (feedbackMesh != null)
        {
            if (ConfigurationManager.IsGazeClickEnabled)
            {
                feedbackMesh.text = LanguageManager.GetString("GazeToClimb");
            }
            else
            {
                feedbackMesh.text = LanguageManager.GetString("ClickToClimb");
            }
        }
    }

    protected TextMesh GetBestTextMesh(TextMesh[] meshes, bool precise)
    {
        float rotationDegree = 45f;
        int maxRotationCount = 8;
        if (!precise)
        {
            rotationDegree *= 2;
            maxRotationCount /= 2;
        }

        TextMesh currentlyActiveMesh = null;
        for (int i = 0; i < meshes.Length; i++)
        {
            RaycastHit hit;
            if (meshes[i].gameObject.activeSelf)
            {
                currentlyActiveMesh = meshes[i];
            }

            meshes[i].gameObject.SetActive(true);
            if (!precise && (meshes[i].transform.rotation.eulerAngles.y % 90 != 0))
            {
                meshes[i].transform.rotation = Quaternion.Euler(meshes[i].transform.rotation.eulerAngles.x, meshes[i].transform.rotation.eulerAngles.y - 45, meshes[i].transform.rotation.eulerAngles.z);
            }

            //Debug.DrawRay(MainPlayerController.Instance.transform.position, meshes[i].transform.position - MainPlayerController.Instance.transform.position, Color.green, 1);
            if (Physics.Raycast(MainPlayerController.Instance.transform.position, meshes[i].transform.position - MainPlayerController.Instance.transform.position, out hit) && hit.transform.tag == "FeedbackText")
            {
                TextMesh bestMesh = meshes[i];
                float angleBetweenMeshAndPlayer = Vector3.Angle(bestMesh.transform.forward, Camera.main.transform.forward);
                float bestAngle = angleBetweenMeshAndPlayer;
                float bestAngleRotation = bestMesh.transform.rotation.eulerAngles.y;
                int rotationCount = 0;
                while (rotationCount < maxRotationCount)
                {
                    bestMesh.transform.rotation = Quaternion.Euler(bestMesh.transform.rotation.eulerAngles.x, (bestMesh.transform.rotation.eulerAngles.y + rotationDegree) % 360, bestMesh.transform.rotation.eulerAngles.z);
                    angleBetweenMeshAndPlayer = Vector3.Angle(bestMesh.transform.forward, Camera.main.transform.forward);
                    if (angleBetweenMeshAndPlayer < bestAngle)
                    {
                        bestAngle = angleBetweenMeshAndPlayer;
                        bestAngleRotation = bestMesh.transform.rotation.eulerAngles.y;
                    }
                    rotationCount++;
                }
                bestMesh.GetComponent<Collider>().enabled = false;
                bestMesh.transform.rotation = Quaternion.Euler(bestMesh.transform.rotation.eulerAngles.x, bestAngleRotation, bestMesh.transform.rotation.eulerAngles.z);
                return bestMesh;
            }
            else
            {
                meshes[i].gameObject.SetActive(false);
            }
        }

        if (currentlyActiveMesh != null)
        {
            currentlyActiveMesh.gameObject.SetActive(true);
        }
        return currentlyActiveMesh;
    }

    public void OnPointerExit()
    {
        updateFeedback = false;
        DisableFeedbackTexts();
    }

    private static void DisableFeedbackTexts()
    {
        GameObject[] texts = GameObject.FindGameObjectsWithTag("FeedbackText");
        if (texts != null)
        {
            foreach (GameObject text in texts)
            {
                if (text.GetComponent<Collider>() != null)
                {
                    text.GetComponent<Collider>().enabled = true;
                    text.SetActive(false);
                }
            }
        }
    }
}
