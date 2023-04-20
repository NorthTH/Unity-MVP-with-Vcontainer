using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class RootLifetimeScope : LifetimeScope
{
    [SerializeField]
    GameManager GameManagerPrefab;

    partial void TableRegister(IContainerBuilder builder);
    partial void UserDataRegister(IContainerBuilder builder);
    partial void UtilityRegister(IContainerBuilder builder);

    protected override void Configure(IContainerBuilder builder)
    {
        TableRegister(builder);
        UserDataRegister(builder);
        UtilityRegister(builder);

        builder.Register<ProjectSetting>(Lifetime.Singleton);
        builder.RegisterEntryPoint<ProjectSetting>(Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab<GameManager>(GameManagerPrefab, Lifetime.Singleton).DontDestroyOnLoad();
    }
}