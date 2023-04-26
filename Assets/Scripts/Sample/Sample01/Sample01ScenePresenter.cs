using MVP;

namespace Sample
{
    public sealed class Sample01ScenePresenter : Presenter<Sample01SceneModel, Sample01SceneView>
    {
        public Sample01ScenePresenter(Sample01SceneModel model, Sample01SceneView view) : 
            base(model, view)
        {
        }

        protected override void Initialize() { }
    }
}
