using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System;

public class ScoreSystem : ComponentSystem
{

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<ScorePos>();
    }

    protected override void OnUpdate()
    {
        var scorePosEntity = GetSingletonEntity<ScorePos>();
        var scorePos = EntityManager.GetComponentData<Translation>(scorePosEntity).Value;

        Entities.ForEach((ref GetCollectible getCollectible, ref Translation translation) =>
        {
            var t = getCollectible.t;
            t += Time.DeltaTime;
            EntityManager.SetComponentData<GetCollectible>(getCollectible.entity, new GetCollectible { entity = getCollectible.entity, t = t , score = getCollectible.score});            

            if (math.distance(scorePos, translation.Value) > 1)
            {
                var newPos = math.lerp(translation.Value, scorePos, t);
                EntityManager.SetComponentData<Translation>(getCollectible.entity, new Translation { Value = newPos });
            }
            else
            {                
                GameManagerSystem.Instance.AddScore(getCollectible.score);

                if(getCollectible.score == 100)
                {
                    GameManagerSystem.Instance.ShowTips();
                }

                EntityManager.DestroyEntity(getCollectible.entity);
            }
        });
    }
}
