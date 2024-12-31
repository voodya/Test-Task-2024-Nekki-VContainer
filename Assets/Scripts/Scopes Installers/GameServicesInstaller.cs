using System.Collections.Generic;
using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "GameServicesInstaller", menuName = "Installers/GameScope/GameServicesInstaller")]

public class GameServicesInstaller : ScriptableInstaller
{
    [SerializeField] private List<EnemyView> _enemyViews;
    [SerializeField] private List<GameObject> _obstacles;
    [SerializeField] private GroundView _groundView;
    [SerializeField] private EnemyConfig _enemyConfig;
    [SerializeField] private CharacterView _characterView;
    [SerializeField] private int _enemyCount;

    public override void Install(IContainerBuilder builder)
    {
        builder.RegisterFactory<EnemyConfig, EnemyPresenter>(
            x =>
            config => new EnemyPresenter(Instantiate(GetRandomView()), new EnemyModel(config), x)
            , Lifetime.Scoped);
        builder.RegisterFactory<CharacterSaveData, CharacterPresenter>(
            x =>
            save => new CharacterPresenter(Instantiate(_characterView), new CharacterModel(save), x)
            , Lifetime.Scoped);

        builder.Register<MapGeneratorService>(Lifetime.Scoped)
            .As<IMapGeneratorService>()
            .As<IBootableAsync>()
            .WithParameter("groundView", _groundView)
            .WithParameter("obstacles", _obstacles);

        builder.Register<EnemySpawnService>(Lifetime.Scoped)
            .As<IEnemySpawnService>()
            .As<IBootableAsync>()
            .WithParameter("enemyConfig", _enemyConfig)
            .WithParameter("enemyCount", _enemyCount);

        builder.Register<RuntimeCharacterService>(Lifetime.Scoped)
            .As<IRuntimeCharacterService>()
            .As<IBootableAsync>();

        builder.Register<GameLoopService>(Lifetime.Scoped)
            .As<IGameLoopService>()
            .As<IBootableAsync>();
        
        builder.Register<RbMovementService>(Lifetime.Scoped)
            .As<IRbMovementService>()
            .As<IBootableAsync>();

        builder.Register<PlayerInputService>(Lifetime.Scoped)
            .As<IPlayerInputService>()
            .As<IBootableAsync>();
    }

    private EnemyView GetRandomView()
    {
        return _enemyViews[Random.Range(0, _enemyViews.Count)];
    }
}







public interface ISpell
{
    int Damage { get; }
    int Range { get; }

    int Speed { get; }
}

public interface ICharacter
{
    void GetDamage(float damage);
}




