using System;

namespace CosmosKernel.Commands
{
    public class Help : Command
    {

        public Help(String name) : base(name) { }

        public override String execute(String[] args)
        {
            Console.Clear();
            return "    Commands :                                                       \n" +
                   "          Command     Use.                                           \n" +
                   "        1. help      : Provide help.                                 \n" +
                   "        2. about     : About.                                        \n" +
                   "        3. shutdown  : Shutdown.                                     \n" +
                   "        4. reboot    : Reboot.                                       \n" +
                   "        5. file      : Edit File. Enter 'file help' for mode details \n" +
                   "        6. clear     : Clear Screen.                                 \n";
        }
    }
}
