using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;

namespace UCommerce.Sitefinity.UI.Mvc
{
    /// <summary>
    /// This class extends the <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.SitefinityControllerFactory"/> by  adding additional logic for resolving MVC Containers throught the Windsor container.
    /// </summary>
    public class WindsorControllerFactory : FrontendControllerFactory
    {
        readonly IWindsorContainer container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null && container.Kernel.HasComponent(controllerType))
                return (IController)container.Resolve(controllerType);

            return base.GetControllerInstance(requestContext, controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            container.Release(controller);
        }
    }
}
