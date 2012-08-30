namespace AsynchronousInterfaces
{
    public interface IProgress
    {
        void ProgressMessage(string msg, params object[] args);

        void Progress(int percentage);
    }
}