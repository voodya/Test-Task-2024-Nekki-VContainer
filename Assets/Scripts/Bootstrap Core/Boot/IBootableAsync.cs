using Cysharp.Threading.Tasks;

public interface IBootableAsync
{
    public bool IsBooted { get; set; }   
    public int Priority { get; set; }
    public UniTask Boot();
}
