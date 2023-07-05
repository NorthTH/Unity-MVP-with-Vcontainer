using MVP;
using VContainer;

namespace Sample
{
    public sealed class Sample03ScenePresenter : Presenter<Sample03SceneModel, Sample03SceneView>
    {
        public Sample03ScenePresenter(Sample03SceneModel model, Sample03SceneView view, IObjectResolver container) :
            base(model, view, container)
        {
        }
    }
}
