using MVP;

namespace SampleAdvance
{
    public sealed class SampleAdvance01SceneModel : Model, ISampleAdvance01SceneModel
    {
        public SampleAdvance01SceneModel() { }

        public override void Initialize() 
        {
            SceneDataPack?.SetSceneComplete();
        }
    }

    public interface ISampleAdvance01SceneModel { }
}
