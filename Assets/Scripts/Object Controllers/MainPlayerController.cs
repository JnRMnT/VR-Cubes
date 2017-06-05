using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPlayerController : JMBehaviour
{
    private float movementSpeed = 5f;
    protected static Vector3 cubeSize;
    public static MainPlayerController Instance;
    private static Vector3 destination, rotationChange;
    private static Cube destinationCube;
    public static Vector3 GridPosition;

    private bool isFalling;

    public override void DoStart()
    {
        Instance = this;
        if (ResourceManager.Instance != null)
        {
            cubeSize = GameObjectHelper.CalculateObjectSize(ResourceManager.Instance.CubePrefab.transform.Find("CubeBody").gameObject);
        }

        destination = Vector3.zero;
        isFalling = false;

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        GvrViewer.Instance.Recenter();
        base.DoStart();
    }

    public override void DoFixedUpdate()
    {
        if (GameManager.GameState == GameState.Started)
        {
            if (destination != Vector3.zero)
            {
                transform.position = Vector3.Lerp(transform.position, destination, Time.fixedDeltaTime * movementSpeed);
                if (Vector3.Distance(transform.position, destination) < 5f)
                {
                    transform.position = destination;
                    destination = Vector3.zero;
                    if (destinationCube != null)
                    {
                        destinationCube.GetComponent<CubeInteractions>().OnPointerExit();
                        destinationCube.GetComponent<CubeInteractions>().OnPointerEnter();
                    }
                    destinationCube = null;
                }
            }
            else if (isFalling)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, transform.position + Vector3.down * 10f, Time.fixedDeltaTime * movementSpeed);
                transform.position = newPosition;
            }
        }

        if (PauseButtonController.Instance != null && GameManager.GameState != GameState.Over)
        {
            if (Camera.main.transform.forward.y > 0.9f)
            {
                if (GameManager.CanBePaused())
                {
                    if (!PauseButtonController.Instance.gameObject.activeSelf)
                    {
                        PauseButtonController.AdjustPosition();
                    }
                    PauseButtonController.Instance.ActivateView();
                }
            }
            else if (GameManager.GameState != GameState.Paused)
            {
                PauseButtonController.Instance.DeactivateView();
            }
        }

        base.DoFixedUpdate();
    }

    public override void DoUpdate()
    {
        if (rotationChange != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationChange), Time.deltaTime * movementSpeed);
            GvrViewer.Instance.Recenter();
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(rotationChange)) < 2f)
            {
                transform.rotation = Quaternion.Euler(rotationChange);
                GvrViewer.Instance.Recenter();
                rotationChange = Vector3.zero;
            }
        }
        base.DoUpdate();
    }

    public static void ChangeRotation(int x, int y, int z)
    {
        rotationChange = new Vector3(x, y, z);
    }

    public bool IsBusy()
    {
        return this.isFalling || destination != Vector3.zero;
    }

    public void Climb(Cube cube)
    {
        if (GameManager.GameState == GameState.Started)
        {
            Vector3 direction = Vector3.zero;
            if (Mathf.Abs(Camera.main.transform.forward.x) > Mathf.Abs(Camera.main.transform.forward.z))
            {
                bool isReverse = Camera.main.transform.forward.x < 0;
                int newPosition = (int)GridPosition.x + (isReverse ? -1 : 1);
                if (newPosition < GameManager.GridSize.x && newPosition >= 0)
                {
                    GridPosition.x = newPosition;
                    direction = new Vector3((isReverse ? -1 : 1), 1, 0);
                }
            }
            else
            {
                bool isReverse = Camera.main.transform.forward.z < 0;
                int newPosition = (int)GridPosition.z + (isReverse ? -1 : 1);
                if (newPosition < GameManager.GridSize.z && newPosition >= 0)
                {
                    GridPosition.z = newPosition;
                    direction = new Vector3(0, 1, (isReverse ? -1 : 1));
                }
            }

            destination = transform.position + new Vector3(cubeSize.x * direction.x, cubeSize.y * direction.y, cubeSize.z * direction.z);
            destinationCube = cube;
        }
    }

    public void WalkTowards(Cube cube)
    {
        if (GameManager.GameState == GameState.Started && CanWalkTowards(cube))
        {
            Vector3 direction = GetWalkDirection(cube);
            destination = transform.position + new Vector3(cubeSize.x * direction.x, cubeSize.y * direction.y, cubeSize.z * direction.z);
            destinationCube = cube;
            if (direction.x != 0)
            {
                GridPosition.x += direction.x;
            }
            if (direction.z != 0)
            {
                GridPosition.z += direction.z;
            }
        }
    }

    protected Vector3 GetWalkDirection(Cube cube)
    {
        Vector3 direction = Vector3.zero;
        if (Mathf.Abs(cube.transform.position.x - transform.position.x) >= cubeSize.x ||
            (cube.transform.position.x == transform.position.x && cube.transform.position.z == transform.position.z &&
            Mathf.Abs(Camera.main.transform.forward.x) > (Mathf.Abs(Camera.main.transform.forward.z))))
        {
            //x axis
            bool isReverse = Camera.main.transform.forward.x < 0;
            int newPosition = (int)GridPosition.x + (isReverse ? -1 : 1);
            if (newPosition < GameManager.GridSize.x && newPosition >= 0)
            {
                direction.x = (isReverse ? -1 : 1);
            }
        }

        if (Mathf.Abs(cube.transform.position.z - transform.position.z) >= cubeSize.z ||
            (cube.transform.position.x == transform.position.x && cube.transform.position.z == transform.position.z &&
            Mathf.Abs(Camera.main.transform.forward.z) > (Mathf.Abs(Camera.main.transform.forward.x))))
        {
            //z axis
            bool isReverse = Camera.main.transform.forward.z < 0;
            int newPosition = (int)GridPosition.z + (isReverse ? -1 : 1);
            if (newPosition < GameManager.GridSize.z && newPosition >= 0)
            {
                direction.z = (isReverse ? -1 : 1);
            }
        }

        return direction;
    }

    public static bool CanWalkTowards(Cube cube)
    {
        Vector3 walkDirection = Instance.GetWalkDirection(cube);
        Vector3 walkDestination = Instance.transform.position + new Vector3(cubeSize.x * walkDirection.x, cubeSize.y * walkDirection.y, cubeSize.z * walkDirection.z);
        bool canWalk = destination == Vector3.zero && Instance.transform.position.y + cube.Size.y > walkDestination.y && cube.IsCloseEnoughToWalk();
        if (canWalk && Mathf.Abs(walkDestination.y - Instance.transform.position.y) < cubeSize.y / 3)
        {
            //raycast if occuppied
            Collider[] overlappingColliders = Physics.OverlapBox(walkDestination, cubeSize / 5 * 2);
            if (overlappingColliders != null)
            {
                foreach (Collider overlappingCollider in overlappingColliders)
                {
                    if (canWalk && overlappingCollider.tag != "Floor" && overlappingCollider.gameObject.GetInstanceID() != cube.gameObject.GetInstanceID())
                    {
                        canWalk = false;
                    }
                }
            }
        }

        return canWalk;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.transform.tag == "Cube" && collision.transform.position.z == transform.position.z && collision.transform.position.x == transform.position.x && collision.transform.position.y > transform.position.y))
        {
            //Cube hit from head, Game Over!
            GameManager.GameOver();
        }
        else if ((collision.transform.tag == "Cube" || collision.transform.tag == "Floor") && isFalling && collision.transform.position.z == transform.position.z && collision.transform.position.x == transform.position.x)
        {
            //Stop Falling
            isFalling = false;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if ((collision.transform.tag == "Cube" || collision.transform.tag == "Floor") &&
            (collision.transform.position.z - transform.position.z < cubeSize.z) &&
            (collision.transform.position.x - transform.position.x < cubeSize.x))
        {
            StartCoroutine(CheckDownCollision());
        }
    }

    private IEnumerator CheckDownCollision()
    {
        if (destination != Vector3.zero)
        {
            yield return new WaitUntil(() => { return destination == Vector3.zero; });
        }

        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position, transform.up * -1, cubeSize.y);
        bool isFloored = false;
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].transform.tag == "Cube" || raycastHits[i].transform.tag == "Floor")
            {
                isFloored = true;
            }
        }

        if (!isFloored)
        {
            //Start Falling Again
            isFalling = true;
        }
    }
}
