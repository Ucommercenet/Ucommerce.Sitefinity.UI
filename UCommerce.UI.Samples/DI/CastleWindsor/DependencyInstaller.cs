using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ucommerce.UI.Samples.Mvc.Models;

namespace Ucommerce.UI.Samples.DI.CastleWindsor
{
    /// <summary>
    /// Class containing logic that registers dependencies in <see cref="IWindsorContainer"/> on application startup.
    /// </summary>
    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            InstallControllers(container);

            InstallServices(container);
        }

        private void InstallControllers(IWindsorContainer container)
        {
            container.Register(
                Classes.
                    FromThisAssembly().
                    BasedOn<IController>().
                    If(c => c.Name.EndsWith("Controller")).
                    LifestylePerWebRequest());
        }

        private void InstallServices(IWindsorContainer container)
        {
            container.Register(
                 Component
                 .For<IDiAutoModel>()
                 .ImplementedBy<DiAutoModel>()
                 .LifestylePerWebRequest());
        }
    }
}
