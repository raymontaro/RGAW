using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Transforms;

public class SpawnCardSystem : ComponentSystem
{
    public static SpawnCardSystem Instance;

    //public Entity currentCardsEntity;
    //public DynamicBuffer<Card> currentCardsBuffer;

    //bool spawn = false;
    protected override void OnStartRunning()
    {
        Instance = this;
    }

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Card>();
        RequireSingletonForUpdate<CardPos>();

        //currentCardsEntity = EntityManager.CreateEntity();
        //currentCardsBuffer = EntityManager.AddBuffer<Card>(currentCardsEntity);
    }

    protected override void OnUpdate()
    {
        //var input = World.GetExistingSystem<InputSystem>();
        //if (input.GetKeyDown(KeyCode.Space))
        //{
        //    Spawn();
        //}

               
    }

    public void Spawn()
    {

        //spawn = true;

        //currentCardsBuffer = EntityManager.GetBuffer<Card>(currentCardsEntity);
        //currentCardsBuffer.Clear();

        Entities.ForEach((ref CardEntityComponent cardEntity) =>
        {
            EntityManager.DestroyEntity(cardEntity.entity);
        });

        var cardsEntity = GetSingletonEntity<Card>();
        var cards = EntityManager.GetBuffer<Card>(cardsEntity);




        Random rand = new Random((uint)math.round(System.DateTime.Now.Millisecond) + 1);
        Card temp;


        for (int i = 0; i < 18; i++)
        {
            int r = rand.NextInt(0, 18);
            temp = cards[r];
            cards[r] = cards[i];
            cards[i] = temp;
        }

        for (int i = 0; i < 18; i++)
        {
            cards = EntityManager.GetBuffer<Card>(cardsEntity);

            var cardPossEntity = GetSingletonEntity<CardPos>();
            var cardPoss = EntityManager.GetBuffer<CardPos>(cardPossEntity);

            float3 pos = cardPoss[i].pos;

            var spawnedEntity = EntityManager.Instantiate(cards[i].entity);

            EntityManager.SetComponentData(spawnedEntity, new Translation { Value = pos });

            //currentCardsBuffer = EntityManager.GetBuffer<Card>(currentCardsEntity);
            //currentCardsBuffer.Add(new Card {entity = spawnedEntity });
        }

        //GameManagerSystem.Instance.SetGameState(GameManagerSystem.Gamestate.firstCard);
    }
}
