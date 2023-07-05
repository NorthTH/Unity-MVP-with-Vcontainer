using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MVP;
using VContainer;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Inject] ISceneLoader sceneLoader;

    [SerializeField] GameObject touchBlockObj;
    [SerializeField] GameObject Curtain;

    public void Initialize()
    {
        Debug.Log($"Initialize: GameManager");
        DontDestroyOnLoad(this);

        sceneLoader.SetUp(scene =>
        {
            touchBlockObj.SetActive(true);
        }, scene =>
        {
            touchBlockObj.SetActive(false);
        }, SetShowLoadingCurtain);
    }

    public async UniTask SetShowLoadingCurtain(bool value)
    {
        Curtain.SetActive(value);
        await UniTask.CompletedTask;
    }
}
