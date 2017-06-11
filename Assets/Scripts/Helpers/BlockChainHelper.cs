using System.Collections.Generic;
using UnityEngine;

public class BlockChainHelper
{
    public static bool CheckChains(Cube cube)
    {
        List<Cube> chain = new List<Cube>();
        //Check towards the bottom
        CheckChain(cube, chain, false);
        chain.Add(cube);
        //Check towards the top
        CheckChain(cube, chain, true);
        if (chain.Count >= 3)
        {
            //We have a chain!
            ScoreManager.HandleChain(chain.Count);
            foreach (Cube chainCube in chain)
            {
                chainCube.HandleChain();
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private static void CheckChain(Cube cube, List<Cube> chain, bool towardsUp)
    {
        bool chainContinues = true;
        while (chainContinues)
        {
            Vector3 lastPosition = cube.transform.position;
            Cube lastCube = cube;
            if (chain.Count > 0)
            {
                lastPosition = chain[chain.Count - 1].transform.position;
                lastCube = chain[chain.Count - 1];
            }

            int direction = towardsUp ? 1 : -1;
            lastPosition += new Vector3(0, Cube.Size.y * direction, 0);

            Cube cubeInPosition = FindCubeInPosition(lastCube, lastPosition, direction);
            if (cubeInPosition != null && cubeInPosition.Color == cube.Color)
            {
                chain.Add(cubeInPosition);
            }
            else
            {
                chainContinues = false;
            }
        }
    }

    private static Cube FindCubeInPosition(Cube neighbouringCube, Vector3 position, int direction)
    {
        Collider[] overlappingColliders = Physics.OverlapBox(position, new Vector3(Cube.Size.x / 3, Cube.Size.y / 2, Cube.Size.z / 3), Quaternion.identity);
        for (int i = 0; i < overlappingColliders.Length; i++)
        {
            if (overlappingColliders[i].tag == "Cube" && overlappingColliders[i].gameObject.GetInstanceID() != neighbouringCube.gameObject.GetInstanceID()
                && (overlappingColliders[i].transform.position.y - neighbouringCube.transform.position.y) * direction > 0)
            {
                return overlappingColliders[i].GetComponent<Cube>();
            }
        }

        return null;
    }
}
