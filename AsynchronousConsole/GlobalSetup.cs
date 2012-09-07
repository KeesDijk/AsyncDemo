namespace AsynchronousConsole
{
    using System;
    using System.Collections.Generic;
    using AsynchronousInterfaces;
    using AsynchronousPlayground;
    using AsynchronousReactiveExtensions;
    using AsynchronousSyncronousStart;
    using AsynchronousThreadAPM;
    using AsynchronousTools;
    using AsynchronousTPLDataFlow;
    using Autofac;

    public static class GlobalSetup
    {
        private static readonly Dictionary<DIConfigurationName, Action> Samples =
            new Dictionary<DIConfigurationName, Action>();

        private static IContainer containerfield;

        private const string LogFileNameToUse = @"..\..\..\samplefiles\smallAvatarlogfile.txt";
        //private const string LogFileNameToUse = @"..\..\..\samplefiles\largeAvatarlogfile.txt";

        static GlobalSetup()
        {
            CreateSampleList();
            ConfigureThreads();
        }

        public static void OverRideDIConfiguration(DIConfigurationName configName)
        {
            Samples[configName].Invoke();
        }

        public static TService Resolve<TService>()
        {
            return containerfield.Resolve<TService>();
        }

        private static void ConfigurePlayground()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<PlaygroundRunner>().As<IRunner>();

            containerfield = builder.Build();
        }

        private static void ConfigurePlinq()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<APMThreadRunner>().As<IRunner>();

            containerfield = builder.Build();
        }

        private static void ConfigureRx()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<ReactiveExtensionsRunner>().As<IRunner>();


            containerfield = builder.Build();
        }

        private static void ConfigureSynchronous()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<ConsoleProgress>().As<IProgress>();
            builder.RegisterType<SynchronousRunner>().As<IRunner>().WithParameter(new NamedParameter("sampleLogFileName", LogFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigureTPLDataflow()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<TPLDataFlowRunner>().As<IRunner>();

            containerfield = builder.Build();
        }

        private static void ConfigureTasks()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<APMThreadRunner>().As<IRunner>();

            containerfield = builder.Build();
        }

        private static void ConfigureThreads()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<APMThreadRunner>().As<IRunner>();

            containerfield = builder.Build();
        }

        private static void CreateSampleList()
        {
            Samples.Add(DIConfigurationName.Threads, ConfigureThreads);
            Samples.Add(DIConfigurationName.PLinq, ConfigurePlinq);
            Samples.Add(DIConfigurationName.Synchronous, ConfigureSynchronous);
            Samples.Add(DIConfigurationName.Playground, ConfigurePlayground);
            Samples.Add(DIConfigurationName.TPLDataflow, ConfigureTPLDataflow);
            Samples.Add(DIConfigurationName.Tasks, ConfigureTasks);
            Samples.Add(DIConfigurationName.Rx, ConfigureRx);
        }
    }
}