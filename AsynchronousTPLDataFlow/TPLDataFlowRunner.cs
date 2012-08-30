﻿namespace AsynchronousTPLDataFlow
{
    using System.Threading.Tasks.Dataflow;
    using AsynchronousInterfaces;

    public class TPLDataFlowRunner : IRunner
    {
        private readonly IOutputWriter output;

        public TPLDataFlowRunner(IOutputWriter output)
        {
            this.output = output;
        }
        
        public void Run()
        {
            var ab = new ActionBlock<int>(i => this.Compute(i));

            ab.Post(1);
            ab.Post(2);
            ab.Post(3);
        }

        private void Compute(int i)
        {
            this.output.Write("TplDataflow: {0}", i);
        }
    }
}