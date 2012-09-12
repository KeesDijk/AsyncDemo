namespace AsynchronousTools
{
    using System;
    using System.Threading;
    using AsynchronousInterfaces;
    using SignalR.Client.Hubs;

    public class SignalRProgress : IProgress
    {
        private readonly HubConnection connection;

        private readonly IHubProxy signalRProgressHub;

        private static object signalRLock = new object();

        private int ignoreThreshold = 100;

        private int ignoredCounter = 0;

        public SignalRProgress()
        {
            // Set connection
            this.connection = new HubConnection("http://localhost:7712/");

            // Make proxy to hub based on hub name on server
            this.signalRProgressHub = this.connection.CreateProxy("ProcessingManager");

            // Start connection
            this.connection.Start().ContinueWith(
                task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine(
                                "There was an error opening the connection:{0}", task.Exception.GetBaseException());
                        }
                        else
                        {
                            Console.WriteLine("Connected");
                        }
                    }).Wait();
        }

        ~SignalRProgress()
        {
            this.connection.Stop();
        }

        public void Progress(int percentage)
        {
            this.Progress(percentage, string.Empty);
        }

        public void Progress(int percentage, string msg, params object[] args)
        {
            lock (signalRLock)
            {
                this.ignoredCounter++;
                if (this.ignoredCounter > this.ignoreThreshold || percentage >= 100)
                {
                    this.ignoredCounter = 0;
                    var message = string.Format(msg, args);
                    this.signalRProgressHub.Invoke<string>("UpdateProgress", percentage, message).ContinueWith(
                        task =>
                            {
                                if (task.IsFaulted)
                                {
                                    Console.WriteLine(
                                        "There was an error calling send: {0}", task.Exception.GetBaseException());
                                }
                                else
                                {
                                    Console.WriteLine(task.Result);
                                }
                            });
                }
            }
        }
    }
}