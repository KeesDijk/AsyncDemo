namespace AsynchronousInterfaces
{
    public interface IOutputWriter
    {
        void Write(string message, params object[] args);

        void WriteLine(string message, params object[] args);

        void WriteLine();
    }
}