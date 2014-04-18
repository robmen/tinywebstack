using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("TinyWebStack")]
[assembly: AssemblyDescription("A lightweight web framework for ASP.NET.")]
[assembly: AssemblyCompany("Rob Mensching")]
[assembly: AssemblyProduct("TinyWebStack")]
[assembly: AssemblyCopyright("Copyright ©  2014")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else 
[assembly: AssemblyConfiguration("Release")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3a5cc85c-571e-4919-977f-afeaa0b0b57c")]

[assembly: AssemblyVersion("0.1.2")]
[assembly: AssemblyFileVersion("0.1.2")]
