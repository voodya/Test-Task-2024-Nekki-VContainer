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



