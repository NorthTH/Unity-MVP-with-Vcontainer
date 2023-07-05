using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample04ScenePresenter : Presenter<Sample04SceneModel, Sample04SceneView>
    {
        public Sample04ScenePresenter(Sample04SceneModel model, Sample04SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }
    }
}
