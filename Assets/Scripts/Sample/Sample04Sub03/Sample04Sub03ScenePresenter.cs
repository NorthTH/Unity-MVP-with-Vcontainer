using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample04Sub03ScenePresenter : Presenter<Sample04Sub03SceneModel, Sample04Sub03SceneView>
    {
        public Sample04Sub03ScenePresenter(Sample04Sub03SceneModel model, Sample04Sub03SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }

        protected override void Initialize() { }
    }
}
