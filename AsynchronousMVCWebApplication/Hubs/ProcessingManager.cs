namespace AsynchronousMVCWebApplication.Hubs
{
    using System.Threading;
    using SignalR.Hubs;

    public class ProcessingManager : Hub
    {
        public void TestRun()
        {
            this.Clients.SendMessage("starting");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(100);
                string message = string.Format("Done : {0}", i + 1);
                this.Clients.updateProgress((i + 1) * 10, message);
            }
            this.Clients.SendMessage("done");
        }

        public void SendMessage(string message)
        {
            this.Clients.sendMessage(message);
        }

        public void UpdateProgress(int percentage, string message)
        {
            this.Clients.updateProgress(percentage, message);
        }
    }
}