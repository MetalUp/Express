using System.Reflection;
using System.Runtime.Loader;

namespace CompileServer.Workers;

internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext {
    public SimpleUnloadableAssemblyLoadContext()
        : base(true) { }

    protected override Assembly Load(AssemblyName assemblyName) => null;
}