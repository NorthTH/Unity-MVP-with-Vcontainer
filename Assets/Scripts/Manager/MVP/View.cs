namespace MVP
{
    public interface IView
    {
        /// <summary>
        /// Presenter から渡されたデータを受け取ります。
        /// </summary>
        public void SetUp(ISceneDataPack SceneDataPack);

        /// <summary>
        /// 値の初期化等を行います。
        /// 複雑な処理は AfterBind で行ってください。
        /// </summary>
        public void Initialize();

        /// <summary>
        /// View と Model が紐付けられた後に呼ばれます。
        /// </summary>
        public void PostInitialize();

        /// <summary>
        ///　Presenter経由でのイベントのみにより、Viewを更新するための処理を行います。
        /// </summary>
        public void Refresh();
    }

    public abstract class View : SealedMonoBehaviour, IView
    {
        protected ISceneDataPack SceneDataPack;

        public virtual void SetUp(ISceneDataPack sceneDataPack)
        {
            SceneDataPack = sceneDataPack;
        }

        public abstract void Initialize();

        public virtual void PostInitialize() { }

        public virtual void Refresh() { }
    }
}
