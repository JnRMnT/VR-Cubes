using System;
using System.Collections;
using UnityEngine;

public class GameManager : JMBehaviour
{
    public static GameState GameState;
    public CubeSpawnManager CubeSpawnManager;
    public static GameManager Instance;
    public static Vector3 GridSize;
    private static bool canBePaused;
    public override void DoStart()
    {
        Instance = this;
        StartGame(6, 12, 6);
        base.DoStart();
    }

    public void StartGame(int gridX, int gridY, int gridZ)
    {
        canBePaused = true;
        GridSize = new Vector3(gridX, gridY, gridZ);
        GameObject gameGrid = new GameObject();
        gameGrid.name = "Game Grid";
        gameGrid.transform.position = Vector3.zero;
        Vector3 cubeSize = GameObjectHelper.CalculateObjectSize(ResourceManager.Instance.CubePrefab.transform.Find("CubeBody").gameObject);
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridZ; j++)
            {
                GameObject floorObject = Instantiate(ResourceManager.Instance.CubePrefab, new Vector3(cubeSize.x * i, 0, cubeSize.z * j), Quaternion.identity);
                GameObject floorCubeBody = floorObject.transform.Find("CubeBody").gameObject;
                floorObject.name = "Floor" + (i + 1) + (j + 1);
                floorCubeBody.tag = "Floor";
                floorObject.transform.SetParent(gameGrid.transform, true);
                floorCubeBody.GetComponent<Cube>().isFloor = true;
                floorCubeBody.GetComponent<Renderer>().material = ResourceManager.GetMaterial("Floor");
                if (i == Mathf.RoundToInt(gridX / 2) && j == Mathf.RoundToInt(gridZ / 2))
                {
                    MainPlayerController.Instance.transform.position = new Vector3(cubeSize.x * i, cubeSize.y, cubeSize.z * j);
                    MainPlayerController.GridPosition = new Vector3(i, 0, j);
                }
            }
        }

        GameState = GameState.Started;
        CubeSpawnManager.StartSpawning();
    }

    public static bool CanBePaused()
    {
        return canBePaused;
    }

    public static void ResumeGame()
    {
        GameState = GameState.Started;
        Instance.StartCoroutine(Instance.WaitToActivatePauseAgain());
    }

    public static void GameOver()
    {
        GameState = GameState.Over;
        GameOverScreenController.Show();
    }

    public static void PauseGame()
    {
        GameState = GameState.Paused;
        canBePaused = false;
    }
    
    private IEnumerator WaitToActivatePauseAgain()
    {
        yield return new WaitForSeconds(1f);
        canBePaused = true;
    }
}