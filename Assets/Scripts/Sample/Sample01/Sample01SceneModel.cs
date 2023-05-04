using MVP;

namespace Sample
{
    public sealed class Sample01SceneModel : Model, ISample01SceneModel
    {
        public Sample01SceneModel() { }

        public override void Initialize()
        {
            SetSceneCompleteLoaded();
        }
    }

    public interface ISample01SceneModel { }
}
