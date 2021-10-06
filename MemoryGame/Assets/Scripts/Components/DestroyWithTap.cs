using Unity.Entities;

[GenerateAuthoringComponent]
public struct DestroyWithTap : IComponentData
{
    public Entity entity;
}
