using System.Collections.Generic;
using UnityEngine;
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
        TMP_InputField textInput;

        public override void Initialize()
        {
            menuButton.SetMenuButtonAction(OpenSample04Sub01Scene, OpenSample04Sub02Scene, OpenSample04Sub03Scene, OpenSample04Sub04Scene);
        }

        void OpenSample04Sub01Scene()
        {
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub01, LoadSceneMode.Additive);
        }

        void OpenSample04Sub02Scene()
        {
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub02, LoadSceneMode.Additive);
        }

        void OpenSample04Sub03Scene()
        {
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub03, LoadSceneMode.Additive);
        }

        void OpenSample04Sub04Scene()
        {
            SceneLoader.Instance.LoadScene(SceneName.Sample04Sub04, LoadSceneMode.Additive);
        }

        /////////////////////
        /// Interface API ///
        /////////////////////
        public override void Refresh()
        {
            textInput.text = string.Empty;
        }

        public override void ReceiveData(object data)
        {
            textInput.text = string.Empty;
            switch (data)
            {
                case List<string> textList:  // ReceiveFromSub01
                    textList.ForEach(x => textInput.text += $"{x}\n");
                    break;
            }
        }
    }

    public interface ISample04SceneView { }
}
