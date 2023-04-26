using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVP;
using VContainer;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Inject] ISceneLoader sceneLoader;

    [SerializeField] GameObject Curtain;

    public void Initialize()
    {
        Debug.Log($"Initialize: GameManager");
        DontDestroyOnLoad(this);
        sceneLoader.Initialize();
    }

    public void SetShowLoadingCurtain(bool value)
    {
        Curtain.SetActive(value);
    }
}
