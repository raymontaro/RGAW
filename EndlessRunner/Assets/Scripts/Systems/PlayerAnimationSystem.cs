using Unity.Entities;
using Unity.Tiny;
using Unity.Tiny.Animation;

public class PlayerAnimationSystem : ComponentSystem
{
    public static float minSpeed = 0.5f;
    float maxSpeed = 0.01f;
    public static float currentSpeed = 0.5f;
    int currentIndex = 0;

    float timer = 0f;

    protected override void OnUpdate()
    {
        //if (!LevelChunkMoveSystem.move)
        //{
        //    var playerAnim = GetSingletonEntity<PlayerAnimation>();
        //    TinyAnimation.SelectClip(World, playerAnim, "Death");
        //    TinyAnimation.Play(World, playerAnim);
        //}

        if (GameManagerSystem.Instance.myGameState != GameManagerSystem.Gamestate.play)
            return;

        if (currentSpeed > maxSpeed)
            currentSpeed -= Time.DeltaTime * 0.01f;
        else
            currentSpeed = maxSpeed;

        var spritesEntity = GetSingletonEntity<PlayerSprite>();
        var sprites = EntityManager.GetBuffer<PlayerSprite>(spritesEntity);

        if (timer < currentSpeed)
        {
            timer += Time.DeltaTime;
        }
        else
        {
            timer = 0f;
            if(currentIndex < sprites.Length - 1)
            {
                currentIndex++;
            }
            else
            {
                currentIndex = 0;
            }
        }

        Entities.ForEach((ref PlayerAnimation playerAnimation, ref SpriteRenderer spriteRenderer) =>
        {
            var settingsEntity = GetSingletonEntity<PlayerSprite>();
            var settings = EntityManager.GetBuffer<PlayerSprite>(settingsEntity);

            spriteRenderer.Sprite = settings[currentIndex].entity;
        });
    }

    public static void RestartSpeed()
    {
        currentSpeed = minSpeed;
    }
}
