namespace AsynchronousPlayground
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows.Forms;
    using AsynchronousInterfaces;

    public class RxEvents
    {
        private readonly IOutputWriter output;

        public RxEvents(IOutputWriter output)
        {
            this.output = output;
        }

        public void Run()
        {
            var txt = new TextBox();
            var frm = new Form { Controls = { txt } };

            var ts = Observable.FromEventPattern<EventArgs>(txt, "TextChanged", TaskPoolScheduler.Default);
            var rs =
                (from e in ts select ((TextBox)e.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromSeconds(0.5));

            using (rs.Subscribe(this.Write))
            {
                Application.Run(frm);
            }
        }

        private void Write(string text)
        {
            this.output.WriteLine(text);
        }
    }
}