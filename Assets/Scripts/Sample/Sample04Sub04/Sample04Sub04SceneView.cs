using UnityEngine;
using MVP;
using VContainer;

namespace Sample
{
    public class Sample04Sub04SceneView : View, ISample04Sub04SceneView
    {
        [Inject] IObjectResolver container;

        [SerializeField]
        ReturnButton returnButton;

        public override void Initialize()
        {
            returnButton.OnClick.AddListener(ReturnButton_OnClick);
        }

        void ReturnButton_OnClick()
        {
            var parentPresenter = container.Resolve<ParentPresenter>().Presenter;
            parentPresenter.Refresh();
            _ = HistoryManager.ReturnScene();
        }
    }

    public interface ISample04Sub04SceneView { }
}
