using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample04Sub04ScenePresenter : Presenter<Sample04Sub04SceneModel, Sample04Sub04SceneView>
    {
        public Sample04Sub04ScenePresenter(Sample04Sub04SceneModel model, Sample04Sub04SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }
    }
}
