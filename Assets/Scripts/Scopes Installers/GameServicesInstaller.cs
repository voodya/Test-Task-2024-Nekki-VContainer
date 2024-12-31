using System.Collections.Generic;
using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "GameServicesInstaller", menuName = "Installers/GameScope/GameServicesInstaller")]

public class GameServicesInstaller : ScriptableInstaller
{
    [SerializeField] private List<EnemyView> _enemyViews;
    [SerializeField] private List<GameObject> _obstacles;
    [SerializeField] private List<SpellView> _spellViews;
    [SerializeField] private GroundView _groundView;
    [SerializeField] private CharacterView _characterView;
    [SerializeField] private int _enemyCount;
    [SerializeField] private int _spellsPoolSize;
    [SerializeField] private List<SpellConfig> _spells;
    [SerializeField] private LayerMask _spellMask;
    [SerializeField] private List<EnemyComposeData> _enemyComposeDatas;

    private Dictionary<string, SpellView> _optimizetSpellViews;
    private Dictionary<string, SpellConfig> _optimizetSpellConfigs;



    public override void Install(IContainerBuilder builder)
    {
        _optimizetSpellViews = new Dictionary<string, SpellView>();
        foreach (var view in _spellViews)
        {
            _optimizetSpellViews[view.SpellViewName] = view;
        }
        _optimizetSpellConfigs = new Dictionary<string, SpellConfig>();
        foreach (var view in _spells)
        {
            _optimizetSpellConfigs[view.SpellName] = view;
        }

        EnemyPresenter getEnemyPresenter(IObjectResolver objectResolver)
        {
            var random = GetRandomView();
            return new EnemyPresenter(Instantiate(random.EnemyView), new EnemyModel(random.EnemyConfig), objectResolver);
        }


        builder.RegisterFactory<EnemyPresenter>(x => () =>  getEnemyPresenter(x), Lifetime.Scoped);

        builder.RegisterFactory<CharacterSaveData, CharacterPresenter>(
            x =>
            save => new CharacterPresenter(Instantiate(_characterView), new CharacterModel(save), x)
            , Lifetime.Scoped);
        builder.RegisterFactory<SpellConfig, SpellPresenter>(
            x =>
            spellConfig => new SpellPresenter(Instantiate(_optimizetSpellViews[spellConfig.SpellName]), new SpellModel(spellConfig), x, _spellMask)
            , Lifetime.Scoped);

        builder.Register<MapGeneratorService>(Lifetime.Scoped)
            .As<IMapGeneratorService>()
            .As<IBootableAsync>()
            .WithParameter("groundView", _groundView)
            .WithParameter("obstacles", _obstacles);

        builder.Register<EnemySpawnService>(Lifetime.Scoped)
            .As<IEnemySpawnService>()
            .As<IBootableAsync>()
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

        builder.Register<SpellHolderService>(Lifetime.Scoped)
            .As<ISpellHolderService>()
            .As<IBootableAsync>()
            .WithParameter("spells", _spells)
            .WithParameter("spellPoolSize", _spellsPoolSize);

        builder.Register<GameUiService>(Lifetime.Scoped)
            .As<IGameUIService>()
            .As<IBootableAsync>()
            .WithParameter("configs", _optimizetSpellConfigs);

    }

    private EnemyComposeData GetRandomView()
    {
        return _enemyComposeDatas[Random.Range(0, _enemyComposeDatas.Count)];
    }
}

[System.Serializable]
public class EnemyComposeData
{
    public EnemyConfig EnemyConfig;
    public EnemyView EnemyView;

}









