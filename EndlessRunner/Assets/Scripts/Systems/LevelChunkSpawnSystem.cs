using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(LevelChunkMoveSystem))]
public class LevelChunkSpawnSystem : SystemBase
{
    public static LevelChunkSpawnSystem Instance;

    Random random;
    bool init = false;

    Entity lvChunksEntity;
    DynamicBuffer<LevelChunks> lvChunksBuffer;

    protected override void OnStartRunning()
    {
        Instance = this;
    }

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Spawn>();

        random = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);

        lvChunksEntity = EntityManager.CreateEntity();
        EntityManager.AddBuffer<LevelChunks>(lvChunksEntity);
    }

    protected override void OnUpdate()
    {
        var spawnEntity = GetSingletonEntity<Spawn>();               

        int initPos = -10;

        if (!init)
        {
            for (int i = 0; i < 5; i++)
            {
                var spawnBuffer = EntityManager.GetBuffer<Spawn>(spawnEntity);
                var spawnedEntity = EntityManager.Instantiate(spawnBuffer[0].entity);
                EntityManager.SetComponentData(spawnedEntity, new Translation { Value = new float3(0, 0, initPos) });
                initPos += 10;

                AddLvBuffer(spawnedEntity);
            }

            init = true;
            return;
        }        
    }

    public void SpawnLevelChunk()
    {
        var spawnEntity = GetSingletonEntity<Spawn>();
        var spawnBuffer = EntityManager.GetBuffer<Spawn>(spawnEntity);
        var spawnedEntity = EntityManager.Instantiate(spawnBuffer[random.NextInt(spawnBuffer.Length)].entity);

        float3 pos = EntityManager.GetComponentData<Translation>(lvChunksBuffer[lvChunksBuffer.Length - 1].entity).Value;
        pos += new float3(0, 0, 5);
        EntityManager.SetComponentData(spawnedEntity, new Translation { Value = pos });

        AddLvBuffer(spawnedEntity);
    }

    public void AddLvBuffer(Entity chunk)
    {        
        lvChunksBuffer = EntityManager.GetBuffer<LevelChunks>(lvChunksEntity);
        lvChunksBuffer.Add(new LevelChunks { entity = chunk });
    }

    public void RemoveLvBuffer(int index)
    {        
        lvChunksBuffer = EntityManager.GetBuffer<LevelChunks>(lvChunksEntity);
        lvChunksBuffer.RemoveAt(index);
    }
}
