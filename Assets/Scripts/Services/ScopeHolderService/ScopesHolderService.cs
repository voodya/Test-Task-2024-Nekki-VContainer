using System;
using System.Collections.Generic;
using VContainer;

public interface IScopesHolderService
{
    List<ScriptableInstaller> GetServices(LocalScope scope);
}


public class ScopesHolderService : IScopesHolderService
{
    private Dictionary<LocalScope, List<ScriptableInstaller>> _installersContainer;

    [Inject]
    public ScopesHolderService(Dictionary<LocalScope, List<ScriptableInstaller>> installers)
    {
        _installersContainer = installers;
    }

    //public void RegisterScopeServices<T>(List<ScriptableInstaller> installers) where T : LifetimeScope
    //{
    //    Type serviceType = typeof(T);
    //    if (!_installersContainer.ContainsKey(serviceType))
    //        _installersContainer[serviceType] = installers;
    //    else
    //    {
    //        Debug.LogWarning($"Already registred {serviceType}");
    //    }
    //}

    public List<ScriptableInstaller> GetServices(LocalScope scope)
    {
        if (_installersContainer.ContainsKey(scope))
            return _installersContainer[scope];
        else
        {
            throw new Exception($"No register installers from {scope}");
        }
    }

}



