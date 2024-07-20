using System;
using System.Collections.Generic;

namespace CosmosKernel.Commands
{
    public class CommandManager
    {
        public List<Command> commands;

        public CommandManager()
        {
            this.commands = new List<Command>(8)
            {
                new Help("help"),
                new About("about"),
                new Reboot("reboot"),
                new Shutdown("shutdown"),
                new File("file"),
                new ClearScreen("clear"),
                //new GUIMode("gui")
            };
        }

        public String processInput(String input)
        {

            String[] split = input.Split(' ');

            String label = split[0];

            List<String> args = new List<String>();

            int ctr = 0;

            foreach (String s in split)
            {

                if (ctr != 0)
                    args.Add(s);

                ++ctr;
            }

            foreach (Command cmd in this.commands)
            {
                if (cmd.name == label)
                    return cmd.execute(args.ToArray());
            }

            return "    Your command \"" + label + "\" does not exist";
        }
    }
}
