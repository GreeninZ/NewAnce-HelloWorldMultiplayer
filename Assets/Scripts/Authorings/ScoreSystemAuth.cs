using Unity.Entities;
using UnityEngine;

//Score handler
public class ScoreSystemAuth: MonoBehaviour 
{ 
    public class Baker: Baker<ScoreSystemAuth>
    {
        public override void Bake(ScoreSystemAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddBuffer<ScoreBuffer>(entity);
        }
    }
}
