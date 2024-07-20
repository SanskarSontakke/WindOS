using System;

namespace CosmosKernel.Commands
{
    public class Reboot : Command
    {

        public Reboot(String name) : base(name) { }

        public override String execute(String[] args)
        {
            Console.Clear();
            Console.WriteLine("Rebooting.");
            Cosmos.System.Power.Reboot();
            return "Rebooting";
        }
    }
}
