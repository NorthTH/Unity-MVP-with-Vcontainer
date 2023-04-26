using UnityEngine;
using UnityEngine.UI;
using MVP;

namespace Sample
{
    public class Sample01SceneView : View, ISample01SceneView
    {
        [SerializeField]
        Button openSample02Btn;

        public override void Initialize()
        {
            openSample02Btn.onClick.AddListener(OpenSample02Btn_OnClick);
        }

        void OpenSample02Btn_OnClick()
        {
            SceneLoader.Instance.LoadScene(SceneName.Sample02);
        }
    }

    public interface ISample01SceneView { }
}
