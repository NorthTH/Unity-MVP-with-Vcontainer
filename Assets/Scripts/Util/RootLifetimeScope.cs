using UnityEngine;
using MVP;
using Manager.UserData;
using Manager.TableData;
using VContainer;
using VContainer.Unity;

public sealed partial class RootLifetimeScope : LifetimeScope
{
    [SerializeField]
    GameManager gameManagerPrefab;

    partial void TableRegister(IContainerBuilder builder);
    partial void UserDataRegister(IContainerBuilder builder);
    partial void UtilityRegister(IContainerBuilder builder);

    protected override void Configure(IContainerBuilder builder)
    {
        TableRegister(builder);
        UserDataRegister(builder);
        UtilityRegister(builder);

        builder.Register<UserData>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<TableData>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<SceneLoader>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<ProjectSetting>(Lifetime.Singleton);
        builder.RegisterEntryPoint<ProjectSetting>(Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab<GameManager>(gameManagerPrefab, Lifetime.Singleton);
    }
}