using System;
using UnityEngine;
using VContainer;

namespace MVP
{
    public abstract class Presenter<TModel, TView> : IPresenter
        where TModel : IModel where TView : IView
    {
        protected IObjectResolver Container { get; }
        protected TModel Model { get; }
        protected TView View { get; }

        protected Presenter(TModel model, TView view, IObjectResolver container)
        {
            Model = model;
            View = view;
            Container = container;
        }

        public async void Initialize()
        {
            Debug.Log($"Initialize: {this}");

            ISceneDataPack sceneDataPack = default;
            try
            {
                sceneDataPack = HistoryManager.IsFirstTimeScene() ? HistoryManager.GetFirstTimeSceneDataPack() : Container.Resolve<ISceneDataPack>();
                Model.SetUp(sceneDataPack);
                View.SetUp(sceneDataPack);

                Model.Initialize();
                View.Initialize();

                await Model.LoadResource();

                Bind();

                Model.PostInitialize();
                View.PostInitialize();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                _ = ErrorManager.Instance.ShowError(ex);
            }
            finally
            {
                sceneDataPack?.SetSceneComplete();
            }
        }

        /// <summary>
        /// ModelとViewを関連付ける処理を実行する
        /// </summary>
        protected virtual void Bind() { }

        public virtual void ReceiveData(object data)
        {
            Model.ReceiveData(data);
            View.Refresh();
        }
        public virtual void Refresh()
        {
            View.Refresh();
        }
    }

    public interface IPresenter
    {
        /// <summary>
        /// 外部のコンテナのイベントにより、Presenter経由でdataを受け渡し、モデルでデータを処理し、ビューの画面更新する
        /// </summary>
        void ReceiveData(object data);

        /// <summary>
        /// 外部のコンテナのイベントにより、Presenter経由でビューの画面更新する
        /// </summary>
        void Refresh();
    }
}
