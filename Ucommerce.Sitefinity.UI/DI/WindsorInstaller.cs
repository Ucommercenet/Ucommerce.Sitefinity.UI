using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Web.Mvc;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.Model.Contracts;

namespace UCommerce.Sitefinity.UI.DI
{
    /// <summary>
    /// The class that contains the installation of the components in the Windsor container.
    /// </summary>
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
                Classes.FromThisAssembly().BasedOn<IController>().If(c => c.Name.EndsWith("Controller"))
                    .LifestyleTransient());
        }

        private void InstallServices(IWindsorContainer container)
        {
            container.Register(
                Component
                    .For<IFacetsFilterModel>()
                    .ImplementedBy<FacetsFilterModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IProductModel>()
                    .ImplementedBy<ProductModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<ICategoryModel>()
                    .ImplementedBy<CategoryModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IMiniBasketModel>()
                    .ImplementedBy<MiniBasketModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<ICartModel>()
                    .ImplementedBy<CartModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IAddressModel>()
                    .ImplementedBy<AddressModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IShippingPickerModel>()
                    .ImplementedBy<ShippingPickerModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IPaymentPickerModel>()
                    .ImplementedBy<PaymentPickerModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IConfirmationMessageModel>()
                    .ImplementedBy<ConfirmationMessageModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IConfirmationEmailModel>()
                    .ImplementedBy<ConfirmationEmailModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IBasketPreviewModel>()
                    .ImplementedBy<BasketPreviewModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IReviewsModel>()
                    .ImplementedBy<ReviewsModel>()
                    .LifestyleTransient());

            container.Register(
                Component
                    .For<IAddReviewModel>()
                    .ImplementedBy<AddReviewModel>()
                    .LifestyleTransient());
        }
    }
}