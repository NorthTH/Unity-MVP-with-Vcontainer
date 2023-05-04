using MVP;

namespace Sample
{
    public sealed class Sample02SceneModel : Model, ISample02SceneModel
    {
        public Sample02SceneModel() { }

        public override void Initialize()
        {
            SetSceneCompleteLoaded();
        }
    }

    public interface ISample02SceneModel { }
}
