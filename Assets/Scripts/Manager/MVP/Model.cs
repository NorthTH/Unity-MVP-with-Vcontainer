using Cysharp.Threading.Tasks;

namespace MVP
{
    public interface IModel
    {
        /// <summary>
        /// Presenter から渡されたデータを受け取ります。
        /// </summary>
        public void SetUp(ISceneDataPack SceneDataPack);

        /// <summary>
        /// API への通信、値の初期化等を行います。
        /// 複雑な処理は AfterBind で行ってください。
        /// </summary>
        public void Initialize();

        /// <summary>
        /// シーン遷移時に必要なリソースの読み込みを行います。
        /// </summary>
        public UniTask LoadResource();

        /// <summary>
        /// View と Model が紐付けられた後に呼ばれます。
        /// </summary>
        public void PostInitialize();

        /// <summary>
        /// Presenter経由でのイベントのみにより、Modelにデータを渡すための処理を行います。
        /// データタイプは object で受け取りますが、必要に応じてキャストしてください。
        /// </summary>
        public void ReceiveData(object data);
    }

    public abstract class Model : IModel
    {
        protected ISceneDataPack SceneDataPack;

        public virtual void SetUp(ISceneDataPack sceneDataPack)
        {
            SceneDataPack = sceneDataPack;
        }

        public abstract void Initialize();

        public async virtual UniTask LoadResource()
        {
            await UniTask.CompletedTask;
        }

        public virtual void PostInitialize() { }

        public virtual void ReceiveData(object data) { }
    }
}
