using MVP;

namespace Sample
{
    public sealed class Sample02ScenePresenter : Presenter<Sample02SceneModel, Sample02SceneView>
    {
        public Sample02ScenePresenter(Sample02SceneModel model, Sample02SceneView view) : 
            base(model, view)
        {
        }

        protected override void Initialize() { }
    }
}
