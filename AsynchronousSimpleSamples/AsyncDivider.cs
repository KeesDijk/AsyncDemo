namespace AsynchronousSimpleSamples
{
    using System.Threading.Tasks;

    public static class AsyncDivider
    {
        public static async Task<int> Divide(int numerator, int denominator)
        {
            await TaskEx.Delay(10);
            var retValue = numerator / denominator;
            return retValue;
        }
    }
}