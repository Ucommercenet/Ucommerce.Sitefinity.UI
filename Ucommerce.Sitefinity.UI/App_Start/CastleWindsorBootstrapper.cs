using System;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Ucommerce.Sitefinity.UI.App_Start
{
    public class CastleWindsorBootstrapper : IContainerAccessor
    {
        public CastleWindsorBootstrapper(IWindsorContainer container)
        {
            this.Container = container;
        }

        public IWindsorContainer Container { get; }

        public static CastleWindsorBootstrapper Bootstrap()
        {
            var container = new WindsorContainer().
                Install(FromAssembly.This());

            return new CastleWindsorBootstrapper(container);
        }
    }
}
