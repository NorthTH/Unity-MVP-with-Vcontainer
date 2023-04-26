using MVP;

namespace Sample
{
    public sealed class SampleScenePresenter01 : Presenter<SampleSceneModel01, SampleSceneView01>
    {
        public SampleScenePresenter01(SampleSceneModel01 model, SampleSceneView01 view) :
            base(model, view)
        {
        }

        protected override void Initialize() { }
    }
}
