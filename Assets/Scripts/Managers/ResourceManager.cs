using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : JMBehaviour
{
    public GameObject CubePrefab;
    public static ResourceManager Instance;
    protected Dictionary<string, Material> materials;
    public void Start()
    {
        Instance = this;
        DoStart();
    }

    public override void DoStart()
    {
        materials = new Dictionary<string, Material>();
        Material[] resourceMaterials = Resources.LoadAll<Material>("Materials");
        foreach(Material resourceMaterial in resourceMaterials)
        {
            materials.Add(resourceMaterial.name, resourceMaterial);
        }
        base.DoStart();
    }

    public static Material GetMaterial(string materialName)
    {
        return Instance.materials[materialName];
    }
}
