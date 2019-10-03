using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Web.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces;
using Ucommerce.Sitefinity.UI.Mvc.Model.Interfaces.Impl;

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

            container.Register(
                 Component
                 .For<IMiniBasketService>()
                 .ImplementedBy<MiniBasketService>()
                 .LifestylePerWebRequest());

            container.Register(
                 Component
                 .For<IMiniBasketModel>()
                 .ImplementedBy<MiniBasketModel>()
                 .LifestylePerWebRequest());

            container.Register(
                 Component
                 .For<ICartModel>()
                 .ImplementedBy<CartModel>()
                 .LifestylePerWebRequest());

            container.Register(
                 Component
                 .For<IAddressModel>()
                 .ImplementedBy<AddressModel>()
                 .LifestylePerWebRequest());

            container.Register(
             Component
             .For<IOrderOverviewModel>()
             .ImplementedBy<OrderOverviewModel>()
             .LifestylePerWebRequest());
        }
    }
}
