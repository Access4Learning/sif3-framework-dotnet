using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Sif.Framework.Demo.Au.Consumer")]
[assembly: AssemblyDescription("Consumer created to demonstrate SIF 3 Framework usage.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Systemic Pty Ltd")]
[assembly: AssemblyProduct("Sif.Framework.Demo.Au.Consumer")]
[assembly: AssemblyCopyright("Copyright © Systemic Pty Ltd 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8fda860a-dafb-4b19-ab19-18d586435573")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.0.0")]

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
