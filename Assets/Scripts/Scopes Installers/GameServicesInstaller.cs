using System;
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
    [SerializeField] private List<ComplexSpellConfig> _spells;
    [SerializeField] private LayerMask _spellMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private List<EnemyComposeData> _enemyComposeDatas;

    private Dictionary<string, SpellView> _optimizetSpellViews;
    private Dictionary<string, ComplexSpellConfig> _optimizetSpellConfigs;



    public override void Install(IContainerBuilder builder)
    {
        _optimizetSpellViews = new Dictionary<string, SpellView>();
        foreach (var view in _spellViews)
        {
            _optimizetSpellViews[view.SpellViewName] = view;
        }
        _optimizetSpellConfigs = new Dictionary<string, ComplexSpellConfig>();
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

        RegisterSpellFactorys(builder);

        builder.Register<MapGeneratorService>(Lifetime.Scoped)
            .As<IMapGeneratorService>()
            .As<IBootableAsync>()
            .WithParameter("groundView", _groundView)
            .WithParameter("obstacles", _obstacles);

        builder.Register<EnemySpawnService>(Lifetime.Scoped)
            .As<IEnemySpawnService>()
            .As<IDisposable>()
            .As<IBootableAsync>()
            .WithParameter("enemyCount", _enemyCount);

        builder.Register<RuntimeCharacterService>(Lifetime.Scoped)
            .As<IRuntimeCharacterService>()
            .As<IBootableAsync>();

        builder.Register<GameLoopService>(Lifetime.Scoped)
            .As<IGameLoopService>()
            .As<IBootableAsync>();
        
        builder.Register<RbMovementService>(Lifetime.Scoped)
            .As<IMovementService>()
             .WithParameter("layerMask", _groundMask)
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

    private void RegisterSpellFactorys(IContainerBuilder builder)
    {
        builder.RegisterFactory<ComplexSpellConfig, ABaseSpellPresenter>(resolver => data => {
            return GetPresenter(data, resolver);
        }, Lifetime.Scoped);
    }

    private ABaseSpellPresenter GetPresenter(ComplexSpellConfig data, IObjectResolver resolver)
    {
        try
        {
            switch (data.SpellType)
            {
                case ESpellType.MultiTarget:
                    return new MultiTargetSpellPresenter(GetSpellsViews(_optimizetSpellViews[data.Piece.SpellName], data.Piece.MaxPieceCount), new SpellModel(data.Piece, data.SpellName), resolver, _spellMask);
                case ESpellType.Forward:
                    return new ForwardSpellPresenter(GetSpellsViews(_optimizetSpellViews[data.Piece.SpellName], 1), new SpellModel(data.Piece, data.SpellName), resolver, _spellMask);
                default:
                    throw new NotImplementedException();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Wrong create SpellPresenter" + ex);
            return null;
        }
        
    }

    public List<ISpellView> GetSpellsViews(SpellView pfb, int count)
    {
        List<ISpellView> spells = new(count);
        for (int i = 0; i < count; i++)
        {
            spells.Add(Instantiate(pfb));
        }

        return spells;
    }

    private EnemyComposeData GetRandomView()
    {
        return _enemyComposeDatas[UnityEngine.Random.Range(0, _enemyComposeDatas.Count)];
    }
}

public enum ESpellType
{
    Forward,
    MultiTarget
}


[System.Serializable]
public class EnemyComposeData
{
    public EnemyConfig EnemyConfig;
    public EnemyView EnemyView;

}









