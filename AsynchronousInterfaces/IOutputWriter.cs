namespace AsynchronousInterfaces
{
    public interface IOutputWriter
    {
        void WriteLine(string message, params object[] args);
        void Write(string message, params object[] args);
    }
}