using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(UCommerce.Sitefinity.UI.RazorGeneratorMvcStart), "Start")]

namespace UCommerce.Sitefinity.UI {
    /// <summary>
    /// This class the handle the registration of the Razor View Engine. 
    /// </summary>
    public static class RazorGeneratorMvcStart {
        public static void Start() {
        }
    }
}
