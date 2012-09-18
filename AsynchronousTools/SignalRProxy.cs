namespace AsynchronousTools
{
    using AsynchronousInterfaces;
    using SignalR.Client.Hubs;

    public class SignalRProxy : IProgress, IOutputWriter
    {
        private const int IgnoreThreshold = 10;

        private static readonly object SignalRLock = new object();

        private readonly HubConnection connection;

        private readonly IOutputWriter output = new SimpleConsoleWriter();

        private readonly IHubProxy signalRProgressHub;

        private int highestPercentageSoFar;

        private int ignoredCounter;

        // needs to be injected later
        public SignalRProxy()
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
                            this.output.WriteLine(
                                "There was an error opening the connection:{0}", task.Exception.GetBaseException());
                        }
                        else
                        {
                            this.output.WriteLine("Connected");
                        }
                    }).Wait();
        }

        ~SignalRProxy()
        {
            this.connection.Stop();
        }

        public void Progress(int percentage)
        {
            lock (SignalRLock)
            {
                this.UpdateProgress(percentage, string.Empty, null);
            }
        }

        public void Progress(int percentage, string msg, params object[] args)
        {
            lock (SignalRLock)
            {
                this.UpdateProgress(percentage, msg, args);
            }
        }

        private void UpdateProgress(int percentage, string msg, object[] args)
        {
            if (percentage > this.highestPercentageSoFar)
            {
                this.highestPercentageSoFar = percentage;
                this.ignoredCounter++;
                if (this.ignoredCounter > IgnoreThreshold || percentage >= 100)
                {
                    this.ignoredCounter = 0;
                    var message = string.Format(msg, args);
                    this.signalRProgressHub.Invoke<string>("UpdateProgress", percentage, message).ContinueWith(
                        task =>
                            {
                                if (task.IsFaulted)
                                {
                                    this.output.WriteLine(
                                        "There was an error calling send: {0}", task.Exception.GetBaseException());
                                }
                                else
                                {
                                    this.output.WriteLine(task.Result);
                                }
                            });
                }
            }
        }

        public void Write(string msg, params object[] args)
        {
            lock (SignalRLock)
            {
                var message = string.Format(msg, args);
                this.signalRProgressHub.Invoke<string>("SendMessage", message).ContinueWith(
                    task =>
                        {
                            if (task.IsFaulted)
                            {
                                this.output.WriteLine(
                                    "There was an error calling send: {0}", task.Exception.GetBaseException());
                            }
                            else
                            {
                                this.output.WriteLine(task.Result);
                            }
                        });
            }
        }

        public void WriteLine(string message, params object[] args)
        {
            this.Write(message, args);
        }

        public void WriteLine()
        {
            this.Write(string.Empty);
        }
    }
}