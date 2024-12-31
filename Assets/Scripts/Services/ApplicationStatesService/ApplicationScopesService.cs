using System.Collections.Generic;
using UnityEngine;
using VContainer;

public interface IApplicationScopesService
{
    void RegisterScopeEntryPoint<T>(T state, LocalScope stateScope) where T : IABaseState;
    bool TryChangeScope(LocalScope stateScope);
}


public class ApplicationScopesService : IApplicationScopesService
{
    private readonly Dictionary<LocalScope, IABaseState> _scopes;
    private IABaseState _currentScope;

    [Inject]
    public ApplicationScopesService(IObjectResolver objectResolver)
    {
        _scopes = new Dictionary<LocalScope, IABaseState>();
    }

    public void RegisterScopeEntryPoint<T>(T entryPoint, LocalScope scope) where T : IABaseState
    {
        if (!_scopes.ContainsKey(scope))
        {
            _scopes.Add(scope, entryPoint);
        }
        else
            Debug.LogError($"State {scope} already register");
    }

    public bool TryChangeScope(LocalScope scope)
    {
        if (_scopes.ContainsKey(scope))
        {
            _currentScope?.Exit();
            _currentScope = _scopes[scope];
            _currentScope.Enter(scope);
            return true;
        }
        return false;

    }
}
public enum LocalScope
{
    Bootstrap,
    Menu,
    Game
}
