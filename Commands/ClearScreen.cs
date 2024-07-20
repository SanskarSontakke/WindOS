using System;

namespace CosmosKernel.Commands
{
    public class ClearScreen : Command
    {

        public ClearScreen(String name) : base(name) { }

        public override String execute(String[] args)
        {
            Console.Clear();
            return "";
        }
    }
}
