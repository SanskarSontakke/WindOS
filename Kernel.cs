using Cosmos.System.FileSystem;
using CosmosKernel.Commands;
using System;
using WindOS.Graphics;
using Sys = Cosmos.System;

namespace CosmosKernel
{
    public class Kernel : Sys.Kernel
    {
        public GUI gui;
        public CosmosVFS VFS;
        public CommandManager commandManager;

        void DelayInMS(int ms) // Stops the code for milliseconds and then resumes it (Basically It's delay)
        {
            for (int i = 0; i < ms * 100000; i++)
            {
                ;
                ;
                ;
                ;
                ;
            }
        }

        protected override void BeforeRun()
        {
            this.VFS = new CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(this.VFS);
            this.commandManager = new CommandManager();
            System.Console.Write(DateTime.Now);
            System.Console.Write(DateTime.Today);
            this.gui = new GUI();
        }

        protected override void Run()
        {
            gui.HandleGUIInputs();
            return;

            String response;
            System.Console.WriteLine("\n");
            String input = System.Console.ReadLine();
            response = this.commandManager.processInput(input);

            System.Console.WriteLine(response);
        }
    }
}
