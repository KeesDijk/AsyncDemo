namespace AsynchronousConsole
{
    using System;
    using AsynchronousInterfaces;
    using AsynchronousPlayground;
    using AsynchronousReactiveExtensions;
    using AsynchronousThreadAPM;
    using AsynchronousTools;
    using AsynchronousTPLDataFlow;
    using Autofac;

    public static class GlobalSetup
    {
        private static IContainer containerfield;

        static GlobalSetup()
        {
            var container = ConfigureThreads();
            containerfield = container;
        }

        public static void OverRideDIConfiguration(DIConfigurationName configName)
        {
            IContainer container;
            switch (configName)
            {
                case DIConfigurationName.Threads:
                    container = ConfigureThreads();
                    break;
                case DIConfigurationName.Tasks:
                    container = ConfigureTasks();
                    break;
                case DIConfigurationName.PLinq:
                    container = ConfigurePlinq();
                    break;
                case DIConfigurationName.TPLDataflow:
                    container = ConfigureTPLDataflow();
                    break;
                case DIConfigurationName.Rx:
                    container = ConfigureRx();
                    break;
                case DIConfigurationName.Playground:
                    container = ConfigurePlayground();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("configName");
            }

            containerfield = container;
        }

        public static TService Resolve<TService>()
        {
            return containerfield.Resolve<TService>();
        }

        private static IContainer ConfigurePlayground()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<PlaygroundRunner>().As<IRunner>();

            var container = builder.Build();
            return container;
        }

        private static IContainer ConfigurePlinq()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<APMThreadRunner>().As<IRunner>();

            var container = builder.Build();
            return container;
        }

        private static IContainer ConfigureRx()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<ReactiveExtensionsRunner>().As<IRunner>();

            var container = builder.Build();
            return container;
        }

        private static IContainer ConfigureTPLDataflow()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<TPLDataFlowRunner>().As<IRunner>();

            var container = builder.Build();
            return container;
        }

        private static IContainer ConfigureTasks()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<APMThreadRunner>().As<IRunner>();

            var container = builder.Build();
            return container;
        }

        private static IContainer ConfigureThreads()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<APMThreadRunner>().As<IRunner>();

            var container = builder.Build();
            return container;
        }
    }
}