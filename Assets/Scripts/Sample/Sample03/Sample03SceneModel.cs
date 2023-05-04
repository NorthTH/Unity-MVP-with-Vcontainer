using MVP;

namespace Sample
{
    public sealed class Sample03SceneModel : Model, ISample03SceneModel
    {
        public Sample03SceneModel() { }

        public override void Initialize()
        {
            SceneDataPack?.SetSceneComplete();
        }
    }

    public interface ISample03SceneModel { }

    public class Sample03SceneData
    {
        public string textData;
    }
}
