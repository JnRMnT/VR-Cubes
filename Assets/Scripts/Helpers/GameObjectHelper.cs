using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameObjectHelper
{
    public static Vector3 CalculateObjectSize(GameObject gameObject, bool calculateMainObjectOnly = true)
    {
        Vector3 size = Vector3.zero;
        Collider[] allColliders = null;
        if (calculateMainObjectOnly)
        {
            allColliders = new Collider[] { gameObject.GetComponent<Collider>() };
        }
        else
        {
            allColliders = gameObject.GetComponentsInChildren<Collider>();
        }

        foreach (Collider collider in allColliders)
        {
            if (collider.bounds.size != Vector3.zero)
            {
                size += collider.bounds.size;
            }
            else
            {
                Renderer renderer = collider.gameObject.GetComponent<Renderer>();
                size += renderer.bounds.size;
            }
        }

        return size;
    }

    public static Renderer[] GetObjectRenderers(GameObject gameObject)
    {
        List<Renderer> renderers = new List<Renderer>();
        Collider[] allColliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in allColliders)
        {
            Renderer renderer = collider.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderers.Add(renderer);
            }
        }

        return renderers.ToArray();
    }

    public static bool IsOverlapping(GameObject gameObject, string[] excludedTags)
    {
        return GetOverlappingObjects(gameObject, excludedTags, 3f).Length != 0;
    }

    public static Collider[] GetOverlappingObjects(GameObject gameObject, string[] excludedTags)
    {
        return GetOverlappingObjects(gameObject, excludedTags, 2f);
    }

    public static Collider[] GetOverlappingObjects(GameObject gameObject, string[] excludedTags, float sizeDivider)
    {
        List<Collider> overlappingColliders = new List<Collider>();
        Collider[] allColliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in allColliders)
        {
            Renderer renderer = collider.GetComponent<Renderer>();
            Vector3 center = new Vector3(renderer.bounds.center.x, renderer.bounds.center.y, renderer.bounds.center.z);
            Vector3 size = new Vector3(renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z) / sizeDivider;
            overlappingColliders.AddRange(Physics.OverlapBox(center, size, gameObject.transform.rotation)
                .Where(e => !allColliders.Contains(e) && !(e is TerrainCollider) && !(excludedTags.Contains(e.tag))).ToArray());
        }

        return overlappingColliders.ToArray();
    }
    public static bool IsInCameraView(Vector3 position)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.z > 0;
    }

    public static Vector3 GetCameraViewPosition(Vector3 position)
    {
        return Camera.main.WorldToViewportPoint(position);
    }

    public static GameObject GetSelectedObject(Vector3 position)
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(position), out hitInfo);
        if (hit)
        {
            return hitInfo.transform.gameObject;
        }
        else {
            return null;
        }
    }

    public static Transform FindChildRecursively(Transform parent, string childName)
    {
        if(parent.name == childName)
        {
            return parent;
        }
        else if(parent.childCount > 0)
        {
            foreach(Transform child in parent)
            {
                Transform foundChild = FindChildRecursively(child, childName);
                if(foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }
        else
        {
            return null;
        }
    }
}