namespace AsynchronousThreadEAP
{
    using System.IO;

    public class AsyncFileReadInfo
    {
        public AsyncFileReadInfo(byte[] array, Stream stream)
        {
            this.ByteArray = array;
            this.MyStream = stream;
        }

        public byte[] ByteArray { get; private set; }

        public Stream MyStream { get; private set; }
    }
}