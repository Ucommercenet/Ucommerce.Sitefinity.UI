using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ucommerce.Sitefinity.UI.Mvc.Model;

namespace Ucommerce.Sitefinity.UI.Mvc.Infrastructure
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegisterFactories(container);
            RegisterModels(container);
        }

        private void RegisterFactories(IWindsorContainer container)
        {
            container.AddFacility<TypedFactoryFacility>();
            container.Register(
                 Component
                 .For<IModelFactory>()
                 .AsFactory());
        }

        private void RegisterModels(IWindsorContainer container)
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
