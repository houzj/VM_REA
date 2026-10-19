using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Loader;
using System.Security.Cryptography;
using VisionPluginContracts;

namespace VisionEditor;

// An explicit host reference, not a vendor manifest or a parameter framework.
public sealed record ManagedToolPluginReference(string AssemblyPath,string TypeFullName,string Sha256,string ModuleId,string Version);
public sealed record LoadedToolPlugin(ManagedToolPluginReference Reference,IVisionToolPlugin Plugin,string AssemblySha256,string ModuleVersionId);

/// <summary>Loads only an explicitly selected implementation of the existing public plugin contract.</summary>
/// <remarks>Selected code runs with host permissions. Interface metadata is not a sandbox.</remarks>
public static class ManagedToolPluginLoader
{
    public static LoadedToolPlugin Load(ManagedToolPluginReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        if(!Path.IsPathFullyQualified(reference.AssemblyPath)||string.IsNullOrWhiteSpace(reference.TypeFullName)||string.IsNullOrWhiteSpace(reference.ModuleId)||string.IsNullOrWhiteSpace(reference.Version))
            throw new ArgumentException("An absolute selected assembly, exact type and module identity are required.");
        string path=Path.GetFullPath(reference.AssemblyPath);
        byte[] bytes=File.ReadAllBytes(path);
        string digest=Convert.ToHexString(SHA256.HashData(bytes));
        if(!string.Equals(digest,reference.Sha256,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Selected plugin SHA256 does not match.");
        string assemblyName;Guid mvid;
        using(var pe=new PEReader(new MemoryStream(bytes,false)))
        {
            if(!pe.HasMetadata)throw new InvalidDataException("Selected plugin has no managed metadata.");
            var metadata=pe.GetMetadataReader();
            if(!metadata.IsAssembly)throw new InvalidDataException("Selected plugin must be a managed assembly.");
            assemblyName=metadata.GetString(metadata.GetAssemblyDefinition().Name);mvid=metadata.GetGuid(metadata.GetModuleDefinition().Mvid);
            var selected=metadata.TypeDefinitions.Where(h=>FullName(metadata,h)==reference.TypeFullName).ToArray();
            if(selected.Length!=1)throw new InvalidDataException("Exact selected plugin type was not found.");
            var type=metadata.GetTypeDefinition(selected[0]);
            if((type.Attributes&TypeAttributes.VisibilityMask)!=TypeAttributes.Public||(type.Attributes&(TypeAttributes.Abstract|TypeAttributes.Interface))!=0||type.GetGenericParameters().Count!=0)
                throw new InvalidDataException("Selected plugin must be a public concrete non-generic type.");
            bool compatible=type.GetInterfaceImplementations().Select(h=>metadata.GetInterfaceImplementation(h).Interface).Any(h=>IsCurrentContract(metadata,h));
            if(!compatible)throw new InvalidDataException("Selected type must directly declare the current IVisionToolPlugin contract.");
        }
        // Do not install a DLL search hook or scan/execute neighboring vendor libraries.
        // Dependencies must already be provided by the host's ordinary resolution.
        var existing=AssemblyLoadContext.Default.Assemblies.FirstOrDefault(a=>a.GetName().Name==assemblyName);
        if(existing is not null&&(string.IsNullOrEmpty(existing.Location)||!string.Equals(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(existing.Location))),digest,StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A different plugin assembly with the same simple name is already loaded.");
        var assembly=existing??AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        if(assembly.ManifestModule.ModuleVersionId!=mvid||!string.Equals(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(assembly.Location))),digest,StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Loaded plugin identity differs from selected bytes.");
        var runtimeType=assembly.GetType(reference.TypeFullName,throwOnError:true)!;
        if(!typeof(IVisionToolPlugin).IsAssignableFrom(runtimeType)||runtimeType.GetConstructor(Type.EmptyTypes) is null)
            throw new InvalidDataException("Runtime plugin contract or public parameterless constructor is incompatible.");
        var plugin=(IVisionToolPlugin)Activator.CreateInstance(runtimeType)!;
        if(plugin.Manifest.Id!=reference.ModuleId||plugin.Manifest.Version!=reference.Version||plugin.Descriptor.Id!=reference.ModuleId||plugin.Descriptor.Version!=reference.Version)
            throw new InvalidDataException("Plugin manifest/descriptor does not match selected module identity.");
        // Validate and freeze the same public descriptor using a private registry,
        // leaving both real registries unchanged until the host commits registration.
        var validation=new VisionFramework.ModuleRegistry();validation.Register(plugin.Descriptor,plugin.CreateRuntime,plugin is IToolNodeDescriptorProvider dynamicProvider?dynamicProvider.DescribeNode:null);
        return new(reference,plugin,digest,mvid.ToString());
    }
    static bool IsCurrentContract(MetadataReader metadata,EntityHandle handle)
    {
        if(handle.Kind!=HandleKind.TypeReference)return false;
        var reference=metadata.GetTypeReference((TypeReferenceHandle)handle);
        if(metadata.GetString(reference.Name)!=nameof(IVisionToolPlugin)||metadata.GetString(reference.Namespace)!=typeof(IVisionToolPlugin).Namespace||reference.ResolutionScope.Kind!=HandleKind.AssemblyReference)return false;
        var assembly=metadata.GetAssemblyReference((AssemblyReferenceHandle)reference.ResolutionScope);
        var host=typeof(IVisionToolPlugin).Assembly.GetName();
        return metadata.GetString(assembly.Name)==host.Name&&assembly.Version==host.Version
            &&metadata.GetString(assembly.Culture)==(host.CultureName??"")
            &&metadata.GetBlobBytes(assembly.PublicKeyOrToken).SequenceEqual(host.GetPublicKeyToken()??[]);
    }
    static string FullName(MetadataReader metadata,TypeDefinitionHandle handle)
    {
        var type=metadata.GetTypeDefinition(handle);string name=metadata.GetString(type.Name);
        var declaring=type.GetDeclaringType();if(!declaring.IsNil)return FullName(metadata,declaring)+"+"+name;
        string ns=metadata.GetString(type.Namespace);return string.IsNullOrEmpty(ns)?name:ns+"."+name;
    }
}
