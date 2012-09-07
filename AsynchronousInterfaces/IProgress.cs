namespace AsynchronousInterfaces
{
    public interface IProgress
    {
        void Progress(int percentage, string msg, params object[] args);

        void Progress(int percentage);
    }
}