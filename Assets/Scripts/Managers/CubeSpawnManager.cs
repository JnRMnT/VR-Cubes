using UnityEngine;

public class CubeSpawnManager : JMBehaviour
{
    private float minSpawnInterval = 1f;
    private float initialSpawnInterval = 3f;
    private float spawnTimer;
    private Vector3 cubeSize;
    private bool spawningActive;

    public override void DoStart()
    {
        base.DoStart();
    }

    public void StartSpawning()
    {
        cubeSize = GameObjectHelper.CalculateObjectSize(ResourceManager.Instance.CubePrefab.transform.Find("CubeBody").gameObject);
        spawningActive = true;
        spawnTimer = 0f;
    }

    public void StopSpawning()
    {
        spawningActive = false;
    }

    public override void DoFixedUpdate()
    {
        if (spawningActive && GameManager.GameState == GameState.Started)
        {
            spawnTimer += Time.fixedDeltaTime;
            if (spawnTimer > GetCurrentSpawnInterval())
            {
                spawnTimer = 0;
                Spawn();
            }
        }
        base.DoFixedUpdate();
    }

    private float GetCurrentSpawnInterval()
    {
        float spawnInterval = initialSpawnInterval - (ScoreManager.Score / 25) * 0.1f;
        if (spawnInterval < minSpawnInterval)
        {
            spawnInterval = minSpawnInterval;
        }

        return spawnInterval;
    }

    private void Spawn()
    {
        int x = Random.Range(0, (int)(GameManager.GridSize.x - 1));
        int z = Random.Range(0, (int)(GameManager.GridSize.z - 1));
        GameObject spawnedCube = Instantiate(ResourceManager.Instance.CubePrefab, new Vector3(cubeSize.x * x, cubeSize.y * (GameManager.GridSize.y + 1), cubeSize.z * z), Quaternion.identity);
        Cube spawnedCubeScript = spawnedCube.transform.Find("CubeBody").GetComponent<Cube>();
        spawnedCubeScript.SetColor(Random.Range(0, (int)CubeColor.Pink));
    }
}
