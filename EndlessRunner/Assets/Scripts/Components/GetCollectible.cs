using Unity.Entities;

[GenerateAuthoringComponent]
public struct GetCollectible : IComponentData
{
    public Entity entity;
    public float t;
    public int score;
}
