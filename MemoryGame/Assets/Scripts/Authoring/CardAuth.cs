using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CardAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject[] Cards;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buffer = dstManager.AddBuffer<Card>(entity);
        for(int i = 0; i < Cards.Length; i++)
        {
            buffer.Add(new Card { entity = conversionSystem.GetPrimaryEntity(Cards[i]) });
        }

        //foreach(var c in Cards)
        //{
        //    buffer.Add(new Card { entity = conversionSystem.GetPrimaryEntity(c) });
        //}
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach(var c in Cards)
        {
            referencedPrefabs.Add(c);
        }
    }
}
