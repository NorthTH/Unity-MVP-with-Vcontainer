using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample04Sub01ScenePresenter : Presenter<Sample04Sub01SceneModel, Sample04Sub01SceneView>
    {
        public Sample04Sub01ScenePresenter(Sample04Sub01SceneModel model, Sample04Sub01SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }

        protected override void Initialize() { }
    }
}
