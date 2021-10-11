using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Scenes;
using Unity.Tiny;
using Unity.Transforms;

public class PlayerCollisionSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<UIElement>();
    }

    protected override void OnUpdate()
    {
        if (GameManagerSystem.Instance.myGameState != GameManagerSystem.Gamestate.play)
            return;

        Entities.ForEach((ref PlayerCollision playerCollision, ref Translation translation, ref Rotation rotation) =>
        {
            ref PhysicsWorld physicsWorld = ref World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

            CollisionFilter coinFilter = new CollisionFilter { BelongsTo = 1 << 3, CollidesWith = 1 << 1 };

            CollisionFilter obstacleFilter = new CollisionFilter { BelongsTo = 1 << 3, CollidesWith = 1 << 2 };

            CollisionFilter cashFilter = new CollisionFilter { BelongsTo = 1 << 3, CollidesWith = 1 << 4 };

            CollisionFilter rFilter = new CollisionFilter { BelongsTo = 1 << 3, CollidesWith = 1 << 5 };

            bool coinTrigger = physicsWorld.BoxCast(translation.Value, rotation.Value, new float3(0.5f, 1f, 0.5f), new float3(0, 0, 1f), 0.01f, out ColliderCastHit coinhit, coinFilter, QueryInteraction.Default);

            bool obstacleTrigger = physicsWorld.BoxCast(translation.Value, rotation.Value, new float3(0.5f, 1f, 0.5f), new float3(0, 0, 1f), 0.01f, out ColliderCastHit obstaclehit, obstacleFilter, QueryInteraction.Default);

            bool cashTrigger = physicsWorld.BoxCast(translation.Value, rotation.Value, new float3(0.5f, 1f, 0.5f), new float3(0, 0, 1f), 0.01f, out ColliderCastHit cashhit, cashFilter, QueryInteraction.Default);

            bool rTrigger = physicsWorld.BoxCast(translation.Value, rotation.Value, new float3(0.5f, 1f, 0.5f), new float3(0, 0, 1f), 0.01f, out ColliderCastHit rhit, rFilter, QueryInteraction.Default);

            if (coinTrigger)
            {
                EntityManager.DestroyEntity(coinhit.Entity);
                GameManagerSystem.Instance.AddScore(10);
            }

            if (obstacleTrigger)
            {                
                GameManagerSystem.Instance.HitObstacle();

                //var uiElementEntity = GetSingletonEntity<UIElement>();
                //var uiElementBuffer = EntityManager.GetBuffer<UIElement>(uiElementEntity);

                //EntityManager.Instantiate(uiElementBuffer[0].entity);                
            }

            if (cashTrigger)
            {
                EntityManager.DestroyEntity(cashhit.Entity);
                GameManagerSystem.Instance.AddScore(50);
            }

            if (rTrigger)
            {
                EntityManager.DestroyEntity(rhit.Entity);
                GameManagerSystem.Instance.AddScore(100);
            }
        });
    }

    
}
