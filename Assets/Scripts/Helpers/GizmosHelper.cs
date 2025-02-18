using System.Collections.Generic;
using UnityEngine;

public interface IGizmosHelper
{
    void AddGizmos(GizmosHelper.DrawFunc drawFunc);

    void RemoveGizmos(GizmosHelper.DrawFunc drawFunc);
}


public class GizmosHelper : MonoBehaviour, IGizmosHelper
{
    public delegate void DrawFunc();
    private List<DrawFunc> _drawFunc = new();

    public void AddGizmos(DrawFunc drawFunc)
    {
        _drawFunc.Add(drawFunc);
    }

    private void OnDrawGizmos()
    {
        foreach (var drawFunc in _drawFunc)
        {
            drawFunc?.Invoke();
        }
    }

    public void RemoveGizmos(DrawFunc drawFunc)
    {
        _drawFunc.Remove(drawFunc);
    }
}
