using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample04Sub02ScenePresenter : Presenter<Sample04Sub02SceneModel, Sample04Sub02SceneView>
    {
        public Sample04Sub02ScenePresenter(Sample04Sub02SceneModel model, Sample04Sub02SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }

        protected override void Initialize() { }
    }
}
