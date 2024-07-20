using System;

namespace CosmosKernel.Commands
{
    public class Shutdown : Command
    {

        public Shutdown(String name) : base(name) { }

        public override String execute(String[] args)
        {
            Console.Clear();
            Console.WriteLine("Shutting down.");
            Cosmos.System.Power.Shutdown();
            return "Shutting down";
        }
    }
}
