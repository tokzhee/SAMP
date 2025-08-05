using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("App.Core")]
[assembly: AssemblyDescription("This is the infrastructure of the app thus keeps the business logic, database services, api services, and other uilities.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Techflare Services LTD")]
[assembly: AssemblyProduct("App.Core")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("Techflare")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("472112fc-9b7c-4b62-bc69-c096ed292e08")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
//Log4net
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
