using System.Collections.Generic;
using UnityEngine;
using MVP;
using TMPro;
using VContainer;

namespace Sample
{
    public class Sample04Sub02SceneView : View, ISample04Sub02SceneView
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
            var peopleData = new StudentData();

            peopleData.name = inputList[0].text;
            int.TryParse(inputList[1].text, out peopleData.age);
            float.TryParse(inputList[2].text, out peopleData.height);
            peopleData.subject = inputList[3].text;

            var parentPresenter = container.Resolve<ParentPresenter>().Presenter;
            parentPresenter.ReceiveData(peopleData);
            _ = HistoryManager.ReturnScene();
        }
    }

    public interface ISample04Sub02SceneView { }
}
