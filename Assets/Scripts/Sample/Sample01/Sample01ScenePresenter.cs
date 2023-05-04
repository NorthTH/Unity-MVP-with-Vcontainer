using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample01ScenePresenter : Presenter<Sample01SceneModel, Sample01SceneView>
    {
        public Sample01ScenePresenter(Sample01SceneModel model, Sample01SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }

        protected override void Initialize() { }
    }
}
