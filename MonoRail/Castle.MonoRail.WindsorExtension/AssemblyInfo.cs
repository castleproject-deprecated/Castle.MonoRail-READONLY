using System;
using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.0.0.0")]

[assembly: CLSCompliant(true)]

#if STRONG
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("../../CastleKey.snk")]
[assembly: AssemblyKeyName("")]
#else
// This assembly must be signed, so if STRONG is not defined
// we assume this compilation is being perfomed by Visual Studio
[assembly: AssemblyKeyFile("../../../../CastleKey.snk")]
#endif
