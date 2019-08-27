using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ucommerce.Sitefinity.UI.Mvc.Model;

namespace Ucommerce.Sitefinity.UI.DI
{
    public class WindsorInstaller : IWindsorInstaller
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
                 .For<IFacetsFilterModel>()
                 .ImplementedBy<FacetsFilterModel>()
                 .LifestylePerWebRequest());

            container.Register(
                 Component
                 .For<IProductModel>()
                 .ImplementedBy<ProductModel>()
                 .LifestylePerWebRequest());

            container.Register(
                 Component
                 .For<ICategoryModel>()
                 .ImplementedBy<CategoryModel>()
                 .LifestylePerWebRequest());
        }
    }
}
