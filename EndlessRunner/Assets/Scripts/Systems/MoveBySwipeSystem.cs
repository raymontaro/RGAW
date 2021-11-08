using Unity.Entities;
using TinyPhysics;
using TinyPhysics.Systems;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(PointerSwipeSystem))]
public class MoveBySwipeSystem : SystemBase
{
    public static MoveBySwipeSystem Instance;

    //enum playerPos { center,right,left};
    enum playerPos { right, left };

    playerPos currentPos = playerPos.left;

    //float3 centerPos = new float3(0, 1, -2);
    float3 rightPos = new float3(1.5f, 1, -2);
    float3 leftPos = new float3(-1.5f, 1, -2);

    protected override void OnStartRunning()
    {
        Instance = this;
    }


    protected override void OnUpdate()
    {
        if (GameManagerSystem.Instance.myGameState != GameManagerSystem.Gamestate.play)
            return;

        Entities.ForEach((ref Swipeable swipeable, ref MoveBySwipe moveBySwipe) =>
        {

            switch (currentPos)
            {
                //case playerPos.center:
                //    if (swipeable.SwipeDirection == SwipeDirection.Right)
                //    {
                //        EntityManager.SetComponentData(moveBySwipe.entity, new Translation { Value = rightPos});
                //        currentPos = playerPos.right;
                //    }
                //    else if (swipeable.SwipeDirection == SwipeDirection.Left)
                //    {
                //        EntityManager.SetComponentData(moveBySwipe.entity, new Translation { Value = leftPos });
                //        currentPos = playerPos.left;
                //    }
                //    break;
                case playerPos.right:
                    if (swipeable.SwipeDirection == SwipeDirection.Left)
                    {
                        EntityManager.SetComponentData(moveBySwipe.entity, new Translation { Value = leftPos });
                        currentPos = playerPos.left;
                    }
                    break;
                case playerPos.left:
                    if (swipeable.SwipeDirection == SwipeDirection.Right)
                    {
                        EntityManager.SetComponentData(moveBySwipe.entity, new Translation { Value = rightPos });
                        currentPos = playerPos.right;
                    }
                    break;                
            }

            // Consume swipe
            swipeable.SwipeDirection = SwipeDirection.None;
        }).WithoutBurst().Run();
    } 
    
    public void Respawn()
    {
        Entities.ForEach((ref Swipeable swipeable, ref MoveBySwipe moveBySwipe) =>
        {

            switch (currentPos)
            {
                case playerPos.right:
                    EntityManager.SetComponentData(moveBySwipe.entity, new Translation { Value = leftPos });
                    currentPos = playerPos.left;                    
                    break;
                case playerPos.left:
                    EntityManager.SetComponentData(moveBySwipe.entity, new Translation { Value = rightPos });
                    currentPos = playerPos.right;                    
                    break;
            }                        
        }).WithoutBurst().Run();
    }
}
