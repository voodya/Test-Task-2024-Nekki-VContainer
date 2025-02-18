using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using VContainer;

public class MultiTargetSpellPresenter : ABaseSpellPresenter
{
    private List<ISpellView> _views;

    public MultiTargetSpellPresenter(List<ISpellView> views, SpellModel model, IObjectResolver objectResolver, LayerMask mask)
        : base(views, model, objectResolver, mask)
    {
        _views = views;
    }


    public override void Throw(CharacterPresenter character)
    {
        Vector3 startPose = character.AttackPose;
        _disposable = new CompositeDisposable();
        List<Transform> targetPoints
            = Physics.OverlapSphere(character.CharacterTransform.position, _model.CastRange, _mask)
            .ToList()
            .Select(x => x.transform)
            .ToList();
        _service.SetCoolDown(this);
        if (targetPoints.Count() <= _views.Count)
        {
            for (int i = 0; i < targetPoints.Count(); i++)
            {
                _views[i].GameObject.SetActive(true);
                _views[i].Rigidbody.isKinematic = true;
                _views[i].Rigidbody.transform.DOMove(targetPoints[i].position, 1f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    _views[i].GameObject.SetActive(false);
                    _service.ReleaseSpell(this);
                });
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        for (int i = 0; i < _views.Count; i++)
        {
            MonoBehaviour.Destroy(_views[i].GameObject);
        }
    }

}
