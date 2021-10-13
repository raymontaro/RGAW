using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerSpritesAuth : MonoBehaviour,IConvertGameObjectToEntity
{
    public Sprite[] sprites;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buffer = dstManager.AddBuffer<PlayerSprite>(entity);
        foreach (var s in sprites)
        {
            buffer.Add(new PlayerSprite { entity = conversionSystem.GetPrimaryEntity(s) });
        }
    }
}

[UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
class DeclareAsteroidSpriteReference : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((PlayerSpritesAuth mgr) =>
        {
            if (mgr.sprites == null)
                return;

            foreach (var s in mgr.sprites)
            {
                DeclareReferencedAsset(s);
            }
        });
    }
}