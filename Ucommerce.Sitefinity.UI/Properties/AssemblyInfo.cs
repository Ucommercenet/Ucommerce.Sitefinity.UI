using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using UCommerce.Sitefinity.UI.App_Start;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("UCommerce.Sitefinity.UI")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("UCommerce")]
[assembly: AssemblyProduct("UCommerce.Sitefinity.UI")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7ffd1a52-79b1-49f8-b155-2beadba87ee0")]

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
[assembly: AssemblyVersion("0.8.0.0")]
[assembly: AssemblyFileVersion("4.0.0.0")]

[assembly: ControllerContainer]
[assembly: ResourcePackage]
[assembly: PreApplicationStartMethod(typeof(Startup), "OnApplicationStart")]

[assembly: WebResource("UCommerce.Sitefinity.UI.assets.dist.css.ucommerce-backend.css", "text/css")]
