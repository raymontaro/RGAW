using Unity.Entities;
using Unity.Mathematics;

public struct Card : IBufferElementData
{
    public Entity entity;
}

public struct CardPos : IBufferElementData
{    
    public float3 pos;
}

public enum GameStates
{
    None,
    Win
}

public struct GameState : IComponentData
{
    public GameStates Value;
}

public struct HudShowState : IComponentData
{
    public GameStates Value;
}

public struct HudObject : IBufferElementData
{
    public Entity Value;
}

public struct Tips : IBufferElementData
{
    public Entity entity;
}