using MVP;

namespace Sample
{
    public sealed class Sample04Sub03SceneModel : Model, ISample04Sub03SceneModel
    {
        public Sample04Sub03SceneModel() { }

        public override void Initialize() 
        {
            SceneDataPack?.SetSceneComplete();
        }
    }

    public interface ISample04Sub03SceneModel { }
}
