using System.Collections.Generic;
using UnityEngine;
using MVP;
using TMPro;
using VContainer;

namespace Sample
{
    public class Sample04Sub03SceneView : View, ISample04Sub03SceneView
    {
        [Inject] IObjectResolver container;

        [SerializeField]
        List<TMP_InputField> inputList;
        [SerializeField]
        ReturnButton returnButton;

        WorkerData workerData;

        public override void Initialize()
        {
            returnButton.OnClick.AddListener(ReturnButton_OnClick);

            workerData = SceneDataPack.GetData<WorkerData>();

            inputList[0].text = workerData.name;
            inputList[1].text = $"{workerData.age}";
            inputList[2].text = $"{workerData.height}";
            inputList[3].text = workerData.job;
        }

        void ReturnButton_OnClick()
        {
            workerData.name = inputList[0].text;
            int.TryParse(inputList[1].text, out workerData.age);
            float.TryParse(inputList[2].text, out workerData.height);
            workerData.job = inputList[3].text;

            var parentPresenter = container.Resolve<ParentPresenter>().Presenter;
            parentPresenter.ReceiveData(workerData);
            _ = HistoryManager.ReturnScene();
        }
    }

    public interface ISample04Sub03SceneView { }
}
