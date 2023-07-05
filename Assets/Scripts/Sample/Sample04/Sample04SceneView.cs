using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MVP;

namespace Sample
{
    public class Sample04SceneView : View, ISample04SceneView
    {
        [SerializeField]
        MenuButton menuButton;
        [SerializeField]
        Button openSampleAdvance01Btn;
        [SerializeField]
        TMP_InputField textInput;

        public override void Initialize()
        {
            menuButton.SetMenuButtonAction(OpenSample04Sub01Scene, OpenSample04Sub02Scene, OpenSample04Sub03Scene, OpenSample04Sub04Scene);
            openSampleAdvance01Btn.onClick.AddListener(OpenSample05Scene);
            openSampleAdvance01Btn.gameObject.SetActive(false);
        }

        void OpenSample04Sub01Scene()
        {
            // Example Receive List<string>
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub01, LoadSceneMode.Additive);
        }

        void OpenSample04Sub02Scene()
        {
            // Example Receive class
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub02, LoadSceneMode.Additive);
        }

        void OpenSample04Sub03Scene()
        {
            // Example Sent&Receive class
            var workerData = new WorkerData();
            workerData.name = "Sam";
            workerData.age = 35;
            workerData.height = 165;
            workerData.job = "Enginear";
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub03, LoadSceneMode.Additive, workerData);
        }

        void OpenSample04Sub04Scene()
        {
            // Example Call Refresh
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub04, LoadSceneMode.Additive);
        }

        void OpenSample05Scene()
        {
            SceneLoader.Instance.LoadScene(SceneName.SampleAdvance01);
        }

        /////////////////////
        /// Interface API ///
        /////////////////////
        public override void Refresh()
        {
            // ReceiveFromSub04
            openSampleAdvance01Btn.gameObject.SetActive(true);
            textInput.text = "Call Refresh from Sub04Scene : Sample05Button is set Active";
        }

        // public override void ReceiveData(object data)
        // {
        //     textInput.text = string.Empty;
        //     switch (data)
        //     {
        //         case List<string> textList:  // ReceiveFromSub01
        //             textList.ForEach(x => textInput.text += $"{x}\n");
        //             break;
        //         case StudentData studentData:  // ReceiveFromSub02
        //             textInput.text = $"Name: {studentData.name}\nAge: {studentData.age}\nHeight: {studentData.height}\nSubject: {studentData.subject}";
        //             break;
        //         case WorkerData workerData:  // ReceiveFromSub03
        //             textInput.text = $"Name: {workerData.name}\nAge: {workerData.age}\nHeight: {workerData.height}\nJob: {workerData.job}";
        //             break;
        //     }
        // }
    }

    public interface ISample04SceneView { }

    public class StudentData : PeopleData
    {
        public string subject;
    }

    public class WorkerData : PeopleData
    {
        public string job;
    }

    public class PeopleData
    {
        public string name;
        public int age;
        public float height;
    }
}
