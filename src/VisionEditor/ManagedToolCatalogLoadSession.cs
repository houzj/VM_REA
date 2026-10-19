using System.IO;

namespace VisionEditor;

public sealed record ManagedToolCatalogLoadObservation(ManagedToolPluginReference Reference,string Stage,int Attempts,Exception? Failure,LoadedToolPlugin? Loaded);
public sealed class ManagedToolCatalogLoadException(ManagedToolPluginReference reference,string stage,Exception inner)
    :InvalidOperationException($"Managed tool '{reference.ModuleId}@{reference.Version}' failed during {stage}: {inner.Message}",inner)
{
    public ManagedToolPluginReference Reference {get;}=reference;
    public string Stage {get;}=stage;
}

/// <summary>One host's explicit catalog references and their real registration outcomes.</summary>
public sealed class ManagedToolCatalogLoadSession
{
    readonly Dictionary<ManagedToolPluginReference,ManagedToolCatalogLoadObservation> observations=new();
    public IReadOnlyList<ManagedToolCatalogLoadObservation> Observations=>observations.Values.ToArray();
    public LoadedToolPlugin EnsureLoaded(ManagedToolPluginReference reference,Action<LoadedToolPlugin> register)
    {
        ArgumentNullException.ThrowIfNull(reference);ArgumentNullException.ThrowIfNull(register);
        reference=reference with{AssemblyPath=Path.IsPathFullyQualified(reference.AssemblyPath)?Path.GetFullPath(reference.AssemblyPath):reference.AssemblyPath,Sha256=reference.Sha256?.ToUpperInvariant()??""};
        if(observations.TryGetValue(reference,out var previous))
        {
            if(previous.Loaded is not null&&previous.Stage=="loaded")return previous.Loaded;
            if(previous.Failure is ManagedToolCatalogLoadException failure)throw failure;
            throw new InvalidOperationException("Managed plugin load is already in progress for this reference.");
        }
        string stage="load";LoadedToolPlugin? loaded=null;
        observations[reference]=new(reference,"loading",1,null,null);
        try
        {
            loaded=ManagedToolPluginLoader.Load(reference);
            stage="register";observations[reference]=new(reference,"registering",1,null,loaded);
            register(loaded);
            observations[reference]=new(reference,"loaded",1,null,loaded);
            return loaded;
        }
        catch(Exception error)
        {
            var failure=new ManagedToolCatalogLoadException(reference,stage,error);
            observations[reference]=new(reference,"failed-"+stage,1,failure,loaded);
            throw failure;
        }
    }
    // Explicit category refresh permits failed references to be attempted again;
    // successful registration belongs to the host lifetime and is never duplicated.
    public void ClearFailures()
    {
        foreach(var key in observations.Where(p=>p.Value.Failure is not null).Select(p=>p.Key).ToArray())observations.Remove(key);
    }
}
