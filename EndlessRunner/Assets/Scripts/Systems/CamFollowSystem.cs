using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Tiny;

public class FollowSystem : SystemBase
{    
    protected override void OnUpdate()
    {
        

        Entities.ForEach((ref Follow Follow, ref Translation translation) =>
        {


            float3 followPos = EntityManager.GetComponentData<Translation>(Follow.entityToFollow).Value;

            float3 pos;

            #region follow
            if (Follow.followX)
                pos.x = followPos.x + Follow.offset.x;
            else
                pos.x = translation.Value.x;

            if (Follow.followY)
                pos.y = followPos.y + Follow.offset.y;
            else
                pos.y = translation.Value.y;

            if (Follow.followZ)
                pos.z = followPos.z + Follow.offset.z;
            else
                pos.z = translation.Value.z;
            #endregion
            
            float3 finalPos = pos;

            #region clamp
            if (Follow.clampLeft && finalPos.x < Follow.leftClampValue)
                finalPos.x = Follow.leftClampValue;

            if (Follow.clampRight && finalPos.x > Follow.rightClampValue)
                finalPos.x = Follow.rightClampValue;

            if (Follow.clampTop && finalPos.x > Follow.topClampValue)
                finalPos.y = Follow.topClampValue;

            if (Follow.clampBottom && finalPos.y < Follow.bottomClampValue)
                finalPos.y = Follow.bottomClampValue;
            #endregion

            translation.Value = finalPos;

            

        }).WithoutBurst().Run();
    }

    
    
}
