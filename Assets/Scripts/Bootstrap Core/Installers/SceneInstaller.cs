using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SceneInstaller : LifetimeScope
{
    [SerializeField] private List<ScriptableInstaller> _installers;

    protected override void Configure(IContainerBuilder builder)
    {
        foreach (var installer in _installers)
        {
            installer.Install(builder);
        }
    }

}
