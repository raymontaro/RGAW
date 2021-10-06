using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class UIauth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject[] Prefabs;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buffer = dstManager.AddBuffer<UIElement>(entity);
        foreach (var s in Prefabs)
        {
            buffer.Add(new UIElement { entity = conversionSystem.GetPrimaryEntity(s) });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var s in Prefabs)
        {
            referencedPrefabs.Add(s);
        }
    }
}