namespace VisionEditor;

// Source-guided ToolBoxData.UpdateLoad state, independent of DLL registration.
// Initialization is deliberately committed before enumeration; a failed attempt
// is retained until the owner replaces this category during explicit refresh.
public sealed class ToolCategoryLoadState<T>
{
    readonly List<T> items=[];
    public bool IsInitialized {get;private set;}
    public int Attempts {get;private set;}
    public Exception? Failure {get;private set;}
    public IReadOnlyList<T> Items=>items;
    public void EnsureLoaded(Func<IEnumerable<T>> load)
    {
        if(IsInitialized)return;
        IsInitialized=true;Attempts++;
        try {foreach(var item in load())items.Add(item);}
        catch(Exception error){Failure=error;throw;}
    }
}
