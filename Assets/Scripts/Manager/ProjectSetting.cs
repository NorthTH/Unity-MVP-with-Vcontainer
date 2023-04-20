using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectSetting : IStartable
{
    public void Start()
    {
        // Vcontainer Initializer
        Debug.Log("Start Vcontainer");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        // Project Initializer
        // GameObject OverScene = GameObject.Instantiate(Resources.Load("OverScene")) as GameObject;
        // GameObject.DontDestroyOnLoad(OverScene);
    }
}
