using System.Collections;
using UnityEngine;

public class Cube : JMBehaviour
{
    private float minFallSpeed = 5f;
    private float maxFallSpeed = 75f;
    public bool isFloor;
    protected float maxPushDistance = 60f;
    protected float maxWalkDistance = 125f;
    public CubeColor Color;
    private bool isFalling, isBeingPushed;
    private Vector3 pushDestination;
    public Vector3 Size;

    public bool IsBusy
    {
        get
        {
            return isFalling || isBeingPushed;
        }
    }

    public override void DoStart()
    {
        if (!isFloor)
        {
            isFalling = true;
        }
        isBeingPushed = false;
        Size = GameObjectHelper.CalculateObjectSize(gameObject);
        base.DoStart();
    }

    public override void DoFixedUpdate()
    {
        if (GameManager.GameState == GameState.Started)
        {
            if (isBeingPushed)
            {
                Vector3 newPosition = Vector3.Lerp(transform.parent.position, pushDestination, Time.fixedDeltaTime * 5f);
                transform.parent.position = newPosition;
                if (Vector3.Distance(transform.parent.position, pushDestination) < 1f)
                {
                    transform.parent.position = pushDestination;
                    pushDestination = Vector3.zero;
                    isBeingPushed = false;
                    GetComponent<CubeInteractions>().OnPointerExit();
                    GetComponent<CubeInteractions>().OnPointerEnter();
                    //Fall Check
                    RaycastHit[] raycastHits = Physics.RaycastAll(transform.parent.position, transform.parent.up * -1, Size.y / 2);
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
                        //if no block is under, start falling again
                        isFalling = true;
                    }
                }
            }
            else if (isFalling)
            {
                Vector3 newPosition = Vector3.Lerp(transform.parent.position, transform.parent.position + Vector3.down * 10f, Time.fixedDeltaTime * CalculateFallSpeed());
                transform.parent.position = newPosition;
            }
        }
        base.DoFixedUpdate();
    }

    private float CalculateFallSpeed()
    {
        float fallSpeed = minFallSpeed + (ScoreManager.Score / 25) * 0.2f;
        if (fallSpeed > maxFallSpeed)
        {
            fallSpeed = maxFallSpeed;
        }

        return fallSpeed;
    }

    public bool IsCloseEnoughToPush()
    {
        if (!isFalling && !isBeingPushed && Vector3.Distance(MainPlayerController.Instance.transform.position, transform.parent.position) < maxPushDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsCloseEnoughToWalk()
    {
        if (!isFalling && !isBeingPushed && Vector3.Distance(MainPlayerController.Instance.transform.position, transform.parent.position) < maxWalkDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanBeClimbed()
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position, transform.up, Size.y);
        bool canBeClimbed = true;
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].transform.tag == "Cube")
            {
                canBeClimbed = false;
            }
        }

        return canBeClimbed;
    }

    public bool CanBePushed()
    {
        Vector3 direction = Vector3.zero;
        if (Mathf.Abs(Camera.main.transform.forward.x) > Mathf.Abs(Camera.main.transform.forward.z))
        {
            bool isReverse = Camera.main.transform.forward.x < 0;
            direction = new Vector3((isReverse ? -1 : 1), 0, 0);
        }
        else
        {
            bool isReverse = Camera.main.transform.forward.z < 0;
            direction = new Vector3(0, 0, (isReverse ? -1 : 1));
        }

        //Check if has any other cube on the way
        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position, direction, Size.y);
        bool canBePushed = true;
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].transform.tag == "Cube")
            {
                canBePushed = false;
            }
        }

        //Check if any other cube is falling on top
        raycastHits = Physics.RaycastAll(transform.position + new Vector3(Size.x * direction.x, 0, Size.z * direction.z), Vector3.up, Size.y);
        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].transform.tag == "Cube")
            {
                canBePushed = false;
            }
        }

        if (canBePushed)
        {
            //Check if is inside the grid
            raycastHits = Physics.RaycastAll(transform.position + new Vector3(Size.x * direction.x, Size.y * direction.y, Size.z * direction.z), transform.up * -1);
            canBePushed = false;
            for (int i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].transform.tag == "Floor")
                {
                    canBePushed = true;
                }
            }
        }

        return canBePushed;
    }

    public void Push()
    {
        if (CanBePushed() && GameManager.GameState == GameState.Started)
        {
            Vector3 direction = Vector3.zero;
            if (Mathf.Abs(Camera.main.transform.forward.x) > Mathf.Abs(Camera.main.transform.forward.z))
            {
                bool isReverse = Camera.main.transform.forward.x < 0;
                direction = new Vector3((isReverse ? -1 : 1), 0, 0);
            }
            else
            {
                bool isReverse = Camera.main.transform.forward.z < 0;
                direction = new Vector3(0, 0, (isReverse ? -1 : 1));
            }

            pushDestination = transform.position + new Vector3(Size.x * direction.x, Size.y * direction.y, Size.z * direction.z);
            isBeingPushed = true;
        }
    }

    public void SetColor(int color)
    {
        Color = (CubeColor)color;
        GetComponent<Renderer>().material = ResourceManager.GetMaterial(Color.ToString());
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.transform.tag == "Cube" || collision.transform.tag == "Floor")
            && !isFloor && isFalling
            && collision.transform.position.z == transform.parent.position.z && collision.transform.position.x == transform.parent.position.x)
        {
            //Stop
            isFalling = false;
            transform.parent.position = new Vector3(transform.parent.position.x, collision.transform.position.y + Size.y, transform.parent.position.z);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Cube" && !isFloor && collision.transform.position.y < transform.parent.position.y)
        {
            StartCoroutine(CheckDownCollision(collision.gameObject.GetComponent<Cube>()));
         }
    }

    private IEnumerator CheckDownCollision(Cube cube)
    {
        if (IsBusy)
        {
            yield return new WaitUntil(() => { return !IsBusy; });
        }

        if (cube.IsBusy)
        {
            yield return new WaitUntil(() => { return !IsBusy; });
        }
        
        RaycastHit[] raycastHits = Physics.RaycastAll(transform.parent.position, transform.parent.up * -1, Size.y);
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