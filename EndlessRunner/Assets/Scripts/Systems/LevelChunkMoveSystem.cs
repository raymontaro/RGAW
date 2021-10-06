using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class LevelChunkMoveSystem : ComponentSystem
{
    public static bool move = true;

    float3 speed = new float3(0,0,5f);

    protected override void OnUpdate()
    {
        if (!move)
            return;

        Entities.ForEach((ref LevelChunkComponent levelChunk, ref Translation translation) =>
        {
            var newPos = (translation.Value -= (speed * Time.DeltaTime));
            EntityManager.SetComponentData(levelChunk.entity, new Translation { Value = newPos });

            if(newPos.z < -10)
            {
                //LevelChunkSpawnSystem.Instance.RemoveLvBuffer(0);
                EntityManager.DestroyEntity(levelChunk.entity);
                LevelChunkSpawnSystem.Instance.SpawnLevelChunk();
            }
        });
    }
    
}
