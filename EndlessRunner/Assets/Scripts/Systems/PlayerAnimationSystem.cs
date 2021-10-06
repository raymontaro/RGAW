using Unity.Entities;
using Unity.Tiny.Animation;

public class PlayerAnimationSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (!LevelChunkMoveSystem.move)
        {
            var playerAnim = GetSingletonEntity<PlayerAnimation>();
            TinyAnimation.SelectClip(World, playerAnim, "Death");
            TinyAnimation.Play(World, playerAnim);
        }
    }
}
