using System;

namespace CosmosKernel.Commands
{
    public class About : Command
    {

        public About(String name) : base(name) { }

        public override String execute(String[] args)
        {
            Console.Clear();
            return "About Center : \n" +
                   "    Version : 0.9 Beta \n" +
                   "    Made by : Sanskar \n" +
                   "    Made using : COSMOS\n" +
                   "    Mode : Command Line Interface (CLI) \n";
        }
    }
}
