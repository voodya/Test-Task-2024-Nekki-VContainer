using System.Collections.Generic;
using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "AllServicesInstaller", menuName = "Installers/AllServicesInstaller")]

public class AllServicesInstaller : ScriptableInstaller
{
    [SerializeField] private List<ScriptableInstaller> _allInstallers;
    public override void Install(IContainerBuilder builder)
    {
        foreach (var item in _allInstallers)
        {
            item.Install(builder);
        }
    }
}
