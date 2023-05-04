using UnityEngine;
using UnityEngine.UI;
using MVP;
using TMPro;

namespace Sample
{
    public class Sample02SceneView : View, ISample02SceneView
    {
        [SerializeField]
        TMP_InputField textInput;
        [SerializeField]
        Button openSample03Btn;

        public override void Initialize()
        {
            openSample03Btn.onClick.AddListener(OpenSample03Btn_OnClick);
        }

        void OpenSample03Btn_OnClick()
        {
            var Sample03SceneData = new Sample03SceneData()
            {
                textData = textInput.text
            };
            SceneLoader.Instance.LoadScene(SceneName.Sample03, Sample03SceneData);
        }
    }

    public interface ISample02SceneView { }
}
