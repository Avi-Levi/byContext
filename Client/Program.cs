﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Client.Views.Login;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Client.Views.Shell;
using NConfig;
using Common;
using Client.Views.Product;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run((Form)BuildContainer().Resolve<IShellView>());
        }

        private static IWindsorContainer BuildContainer()
        {
            WindsorContainer container = new WindsorContainer();
            container.Register(Component.For<ILoginView>().ImplementedBy<LoginView>());

            container.Register(Component.For<LoginServiceAdapter>());
            container.Register(Component.For<ShellController>().LifeStyle.Singleton);

            container.Register(Component.For<IShellView>().ImplementedBy<Shell>());
            container.Register(Component.For<IProductView>().ImplementedBy<ProductView>());

            container.Register(Component.For<ProductsServiceAdapter>());

            container.Register(Component.For<ProxyFactory>());
            container.Register(Component.For<LoggerFactory>());

            IConfigurationService configSvc = BuildConfigurationService();
            container.Register(Component.For<IConfigurationService>().Instance(configSvc));

            container.Register(Component.For<IWindsorContainer>().Instance(container));

            return container;
        }
        private static IConfigurationService BuildConfigurationService()
        {
            IConfigurationService configSvc = Configure.With()
                .RuntimeContext(ctx =>
                {
                    ctx.Add(ConfigConstants.Subjects.Environment.Name, ConfigConstants.Subjects.Environment.Dev);
                    ctx.Add(ConfigConstants.Subjects.AppType.Name, ConfigConstants.Subjects.AppType.OnlineClient);
                    ctx.Add(ConfigConstants.Subjects.MachineName.Name, ConfigConstants.Subjects.MachineName.ClientMachine1);
                })
                .AddFromRemoteWCFService()
                .Build();

            return configSvc;
        }
    }
}
