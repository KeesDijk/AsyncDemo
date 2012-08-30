namespace AsynchronousTests
{
    using System;
    using System.Threading.Tasks;
    using AsynchronousSimpleSamples;
    using FluentAssertions;
    using Xunit;

    public class SimpleTests
    {
        [Fact]
        public async Task DividingByZeroThrowsException()
        {
            var exceptionThrown = false;
            try
            {
                await AsyncDivider.Divide(4, 0);
            }
            catch (DivideByZeroException)
            {
                exceptionThrown = true;
            }

            exceptionThrown.Should().BeTrue();
        }

        [Fact]
        public async void DividingByZeroThrowsExceptionFluent()
        {
            Action divideFunction = async () => { await AsyncDivider.Divide(4, 0); };

            divideFunction.ShouldThrow<DivideByZeroException>();
        }

        [Fact]
        public async Task DividingGivesExpectedCorrectResult()
        {
            var result = await AsyncDivider.Divide(4, 2);

            result.Should().Be(2);
        }

        [Fact]
        public void SimpleSetupTest()
        {
            (4 - 2).Should().Be(2);
        }
    }
}