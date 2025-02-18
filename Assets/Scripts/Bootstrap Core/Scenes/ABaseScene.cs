using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;


public abstract class ABaseScene : MonoBehaviour, IDisposable
{
    [SerializeField] private CanvasGroup _canvasGroup;


    public virtual async UniTask Hide(bool force = false)
    {
        if(force)
        {
            _canvasGroup.alpha = 0;
            await UniTask.CompletedTask;   
        }
        else
            await _canvasGroup.DOFade(0f, 0.5f).AsyncWaitForCompletion();
    }

    public virtual async UniTask Show()
    {
        await _canvasGroup.DOFade(1f, 0.5f).AsyncWaitForCompletion();
    }

    public abstract void Dispose();
}
