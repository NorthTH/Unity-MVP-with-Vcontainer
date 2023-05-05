using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MVP;
using TMPro;
using VContainer;

namespace Sample
{
    public class Sample04Sub01SceneView : View, ISample04Sub01SceneView
    {
        [Inject] IObjectResolver container;

        [SerializeField]
        List<TMP_InputField> inputList;
        [SerializeField]
        ReturnButton returnButton;

        public override void Initialize()
        {
            returnButton.OnClick.AddListener(ReturnButton_OnClick);
        }

        void ReturnButton_OnClick()
        {
            var textList = inputList.Select(x => x.text).ToList();
            var parentView = container.Resolve<ParentContainer>().GetIView();
            parentView.ReceiveData(textList);
            _ = HistoryManager.ReturnScene();
        }
    }

    public interface ISample04Sub01SceneView { }
}
