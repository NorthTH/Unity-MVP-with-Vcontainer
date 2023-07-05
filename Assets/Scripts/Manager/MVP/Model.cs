namespace MVP
{
    public interface IModel
    {
        public void SetUp(ISceneDataPack SceneDataPack);
        public void Initialize();
        public void PostInitialize();
        public void ReceiveData(object data);
    }

    public abstract class Model : IModel
    {
        protected ISceneDataPack SceneDataPack;

        public virtual void SetUp(ISceneDataPack SceneDataPack)
        {
            this.SceneDataPack = SceneDataPack;
        }

        public abstract void Initialize();

        public virtual void PostInitialize()
        {
            SceneDataPack.SetSceneComplete();
        }

        public virtual void ReceiveData(object data) { }
    }
}
