using Unity.Entities;
using Unity.Mathematics;

public struct Spawn : IBufferElementData
{
    public Entity entity;
}

public struct UIElement : IBufferElementData
{
    public Entity entity;
}

public struct LevelChunks : IBufferElementData
{
    public Entity entity;    
}

public struct Reminders : IBufferElementData
{
    public Entity entity;
}

public struct PlayerSprite : IBufferElementData
{
    public Entity entity;
}

public struct AudioLibrary : IComponentData
{
}

public enum AudioTypes
{
    None,
    Reminder,
    Coin,
    Wall,
    Tap,
    Swipe,
    StartButton,
    RestartButton
}

public struct AudioObject : IBufferElementData
{
    public AudioTypes Type;
    public Entity Clip;
}