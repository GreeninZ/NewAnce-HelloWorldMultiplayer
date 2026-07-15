using Unity.Entities;

public struct ScoreBuffer : IBufferElementData //I decided to do it in form of some sort of match score
{
    public Entity hostEntity;
    public int hostScore;

    public Entity clientEntity;
    public int clientScore;
}