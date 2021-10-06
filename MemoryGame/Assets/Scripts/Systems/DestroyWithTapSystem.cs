using Unity.Entities;

public class DestroyWithTapSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<DestroyWithTap>().ForEach((ref Tappable tappable, ref DestroyWithTap destroyWithTap) =>
        {
            if (tappable.IsTapped)
            {
                tappable.IsTapped = false;

                EntityManager.DestroyEntity(destroyWithTap.entity);
            }
        });
    }
}
