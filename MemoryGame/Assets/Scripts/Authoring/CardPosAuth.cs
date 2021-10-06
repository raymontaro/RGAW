using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CardPosAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject[] cardPoss;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var bufferPos = dstManager.AddBuffer<CardPos>(entity);
        foreach(var c in cardPoss)
        {
            bufferPos.Add(new CardPos { pos = c.transform.position });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var c in cardPoss)
        {
            referencedPrefabs.Add(c);
        }
    }
}