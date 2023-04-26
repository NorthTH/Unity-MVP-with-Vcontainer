using UnityEngine;
using MVP;

namespace Sample
{
    public class SampleSceneView01 : View, ISampleSceneView01
    {
        public override void Initialize()
        {
            Debug.Log($"Initialize: {this}");
        }
    }

    public interface ISampleSceneView01 { }
}
