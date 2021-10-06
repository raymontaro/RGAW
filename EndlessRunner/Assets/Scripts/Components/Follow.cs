using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Follow : IComponentData
{
    public Entity entityToFollow;

    public float3 offset;
    public bool followX;
    public bool followY;
    public bool followZ;
    public bool clampLeft;
    public bool clampRight;
    public bool clampTop;
    public bool clampBottom;
    public float leftClampValue;
    public float rightClampValue;
    public float topClampValue;
    public float bottomClampValue;

}
