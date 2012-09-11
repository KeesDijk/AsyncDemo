namespace AsynchronousThreadEAP
{
    using System.ComponentModel;

    public class ProcessFileProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private readonly int lastReadLineNr = 1;

        public ProcessFileProgressChangedEventArgs(int latsReadLineNr, int progressPercentage, object userToken)
            : base(progressPercentage, userToken)
        {
            this.lastReadLineNr = latsReadLineNr;
        }

        public int LastReadLineNr
        {
            get
            {
                return this.lastReadLineNr;
            }
        }
    }
}