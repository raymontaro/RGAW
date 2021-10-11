using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class LevelChunkMoveSystem : ComponentSystem
{
    public static LevelChunkMoveSystem Instance;

    //public static bool move = true;

    float3 speed = new float3(0,0,5f);

    protected override void OnStartRunning()
    {
        Instance = this;
    }

    protected override void OnUpdate()
    {
        if (GameManagerSystem.Instance.myGameState != GameManagerSystem.Gamestate.play)
            return;

        Entities.ForEach((ref LevelChunkComponent levelChunk, ref Translation translation) =>
        {
            var newPos = (translation.Value -= (speed * Time.DeltaTime));
            EntityManager.SetComponentData(levelChunk.entity, new Translation { Value = newPos });

            if(newPos.z < -19f)
            {
                LevelChunkSpawnSystem.Instance.RemoveLvBuffer(0);
                EntityManager.DestroyEntity(levelChunk.entity);
                LevelChunkSpawnSystem.Instance.SpawnLevelChunk();
            }
        });
    }

    public void ChangeSpeed(float value)
    {
        speed = new float3(0, 0, value);
    }
    
}
