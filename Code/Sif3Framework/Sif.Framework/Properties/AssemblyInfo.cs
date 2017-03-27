﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Sif.Framework")]
[assembly: AssemblyDescription("Core library of the SIF3 Framework based on SIF Infrastructure 3.2.1")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Systemic Pty Ltd")]
[assembly: AssemblyProduct("Sif.Framework")]
[assembly: AssemblyCopyright("Copyright © Systemic Pty Ltd 2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6d5ef338-c199-4c43-ae36-e7ddc3d1dabc")]

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
[assembly: AssemblyVersion("3.2.1.0")]
[assembly: AssemblyFileVersion("3.2.1.0")]

// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

// Make types and members with internal scope visible to the assembly containing unit tests.
[assembly: InternalsVisibleToAttribute("Sif.Framework.Tests")]