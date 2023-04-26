using UnityEngine;
using MVP;

namespace Sample
{
    public sealed class SampleSceneModel01 : Model, ISampleSceneModel01
    {
        public SampleSceneModel01() { }

        public override void Initialize()
        {
            Debug.Log($"Initialize: {this}");
        }
    }

    public interface ISampleSceneModel01 { }
}
