namespace AsynchronousConsole
{
    using System;
    using System.Collections.Generic;
    using AsynchronousAsyncAwait;
    using AsynchronousInterfaces;
    using AsynchronousPlayground;
    using AsynchronousPLinq;
    using AsynchronousReactiveExtensions;
    using AsynchronousSyncronousStart;
    using AsynchronousTasks;
    using AsynchronousThreadAPM;
    using AsynchronousThreadEAP;
    using AsynchronousTools;
    using AsynchronousTPLDataFlow;
    using Autofac;

    // this isn't production code, this is used only to make demo-ing easier.
    public static class GlobalSetup
    {
        private const string LargeFileNameToUse = @"..\..\..\samplefiles\largeAvatarlogfile.txt";

        private const string SmallLogFileName = @"..\..\..\samplefiles\smallAvatarlogfile.txt";

        private static readonly Dictionary<DIConfigurationName, Action> Samples =
            new Dictionary<DIConfigurationName, Action>();

        private static IContainer containerfield;

        private static string logFileNameToUse = LargeFileNameToUse;

        private static string selectedLogFile = "Large";

        static GlobalSetup()
        {
            CreateSampleList();
            ConfigureThreadsAPM();
        }

        public static string SelectedLogFile
        {
            get
            {
                return selectedLogFile;
            }

            set
            {
                selectedLogFile = value;
            }
        }

        public static void OverRideDIConfiguration(DIConfigurationName configName)
        {
            Samples[configName].Invoke();
        }

        public static TService Resolve<TService>()
        {
            return containerfield.Resolve<TService>();
        }

        public static void ToggleLogFileToUse()
        {
            if (logFileNameToUse.Equals(SmallLogFileName))
            {
                logFileNameToUse = LargeFileNameToUse;
                SelectedLogFile = "Large";
            }
            else
            {
                logFileNameToUse = SmallLogFileName;
                SelectedLogFile = "Small";
            }
        }

        private static ContainerBuilder BuildupConatinerWithBaseComponents()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<LockingConsoleWriter>().As<IOutputWriter>();
            builder.RegisterType<LockingConsoleProgress>().As<IProgress>();
            builder.RegisterType<LockingCountingDictionary>().As<ICountingDictionary>();
            builder.RegisterType<ConsoleCommander>().As<ICommander>();

            return builder;
        }

        private static void ConfigureAsyncAwait()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<AsyncAwaitRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigurePlayground()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<PlaygroundRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigurePlinq()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<PLinqRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigureRx()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<ReactiveExtensionsRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));
            containerfield = builder.Build();
        }

        private static void ConfigureSignalR()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SignalRProxy>().As<IOutputWriter>();
            builder.RegisterType<SignalRProxy>().As<IProgress>();
            builder.RegisterType<LockingCountingDictionary>().As<ICountingDictionary>();

            builder.RegisterType<TasksRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigureSynchronous()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<SynchronousRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigureTPLDataflow()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<TPLDataFlowRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigureTasks()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<TasksRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigureThreadsAPM()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<APMThreadRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void ConfigureThreadsEAP()
        {
            var builder = BuildupConatinerWithBaseComponents();
            builder.RegisterType<EAPThreadRunner>().As<IRunner>().WithParameter(
                new NamedParameter("sampleLogFileName", logFileNameToUse));

            containerfield = builder.Build();
        }

        private static void CreateSampleList()
        {
            Samples.Add(DIConfigurationName.ThreadsAPM, ConfigureThreadsAPM);
            Samples.Add(DIConfigurationName.ThreadsEAP, ConfigureThreadsEAP);
            Samples.Add(DIConfigurationName.PLinq, ConfigurePlinq);
            Samples.Add(DIConfigurationName.Synchronous, ConfigureSynchronous);
            Samples.Add(DIConfigurationName.Playground, ConfigurePlayground);
            Samples.Add(DIConfigurationName.TPLDataflow, ConfigureTPLDataflow);
            Samples.Add(DIConfigurationName.Tasks, ConfigureTasks);
            Samples.Add(DIConfigurationName.AsyncAwait, ConfigureAsyncAwait);
            Samples.Add(DIConfigurationName.Rx, ConfigureRx);
            Samples.Add(DIConfigurationName.SignalR, ConfigureSignalR);
        }
    }
}