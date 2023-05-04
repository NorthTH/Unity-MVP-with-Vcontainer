using UnityEngine;
using UnityEngine.UI;
using MVP;
using TMPro;

namespace Sample
{
    public class Sample03SceneView : View, ISample03SceneView
    {
        [SerializeField]
        TMP_InputField textInput;
        [SerializeField]
        Button openSample04Btn;

        public override void Initialize()
        {
            openSample04Btn.onClick.AddListener(OpenSample04Btn_OnClick);

            textInput.text = SceneDataPack.GetData<Sample03SceneData>().textData;
        }

        void OpenSample04Btn_OnClick()
        {
            SceneLoader.Instance.LoadScene(SceneName.Sample04);
        }
    }

    public interface ISample03SceneView { }
}
