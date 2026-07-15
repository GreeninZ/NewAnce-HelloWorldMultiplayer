using Unity.Entities;

public struct ScoreBuffer : IBufferElementData //I decided to do it in form of some sort of match score
{
    public int hostScore;
    public int clientScore;
}