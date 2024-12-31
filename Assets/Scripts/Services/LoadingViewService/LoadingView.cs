using TMPro;
using UnityEngine;


public class LoadingView : ABaseScene
{
    [field: SerializeField] public CanvasGroup CanvasGroup { get; set; }
    [field: SerializeField] public TextMeshProUGUI MessageText { get; set; }

    public override void Dispose()
    {
        Debug.LogError("Dispose Loading View (nothing to dispose)");
    }
}
