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