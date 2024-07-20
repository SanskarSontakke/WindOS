using System;
using System.IO;
using Sys = Cosmos.System;

namespace CosmosKernel.Commands
{
    public class File : Command
    {
        string currentPath = "\\";
        int exit = 0;

        public static bool IsCtrlPressed()
        {
            // Check if Ctrl key is pressed
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            return (keyInfo.Modifiers & ConsoleModifiers.Control) != 0;
        }

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

        public File(String name) : base(name) { }

        public override String execute(String[] args)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Cyan;
            System.Console.BackgroundColor = System.ConsoleColor.White;

            String response = "";

        read:
            Console.ReadLine();
            Console.Clear();
        newLine:
            Console.WriteLine("--------------------File Manager--------------------");
            string command = Console.ReadLine();
            if (command == "\n")
                goto newLine;

            switch (command)
            {
                case "mk":
                    try
                    {
                        Console.Write("Enter File Name : ");
                        string fileName = Console.ReadLine();
                        Sys.FileSystem.VFS.VFSManager.CreateFile("0:" + currentPath + fileName);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("File Created Successfully");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error creating file. Error Code : " + ex.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }

                    break;

                case "rm":
                    try
                    {
                        Console.Write("Enter File Name : ");
                        string fileName = Console.ReadLine();
                        if (Sys.FileSystem.VFS.VFSManager.FileExists("0:" + currentPath + fileName))
                        {
                            Sys.FileSystem.VFS.VFSManager.DeleteFile("0:" + currentPath + fileName);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Your file was deleted");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("File " + " does not exist.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error : " + ex.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }

                    break;

                /*
                            case "mkdir":
                                try
                                {
                                    Sys.FileSystem.VFS.VFSManager.CreateDirectory("0:" + currentPath + arg[0]);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    response = "Your Directory was created";
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                catch (Exception ex)
                                {
                                    response = "Error creating directory. Error Code : " + ex.ToString();
                                    break;
                                }

                                break;

                            case "rmdir":
                                try
                                {
                                    if (Sys.FileSystem.VFS.VFSManager.DirectoryExists("0:" + currentPath + arg[0]))
                                    {
                                        Sys.FileSystem.VFS.VFSManager.DeleteDirectory("0:" + currentPath + arg[0], true);
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        response = "Your Directory was removed";
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    response = "Error removing directory. Error Code : " + ex.ToString();
                                    break;
                                }

                                break;

                            case "open":
                                try
                                {
                                    if (Sys.FileSystem.VFS.VFSManager.FileExists("0:" + currentPath + arg[0]))
                                    {
                                        if (arg[1] == "-write")
                                        {
                                            FileStream fs = (FileStream)Sys.FileSystem.VFS.VFSManager.GetFile("0:\\" + currentPath + arg[0]).GetFileStream();
                                            if (fs.CanWrite)
                                            {
                                                Console.Clear();
                                                Console.Write("Welcome to texk editor.\n");
                                                Console.Write("Editing file " + arg[0] + ".\n");
                                                Console.Write("Tip: Type /e\\ to exit and save.\n");
                                                Console.Write("Press enter key to start.");

                                                Console.ReadLine();
                                                Console.ReadLine();
                                                Console.Clear();

                                                String txt = " ";
                                                String datatowrite = " ";
                                            Read:

                                                txt = txt + "\n" + Console.ReadLine();

                                                if ((txt[^1] == '\\') && (txt[^2] == 'e') && (txt[^3] == '/'))
                                                {
                                                    for (global::System.Int32 i = 0; i < 4096; i++)
                                                    {
                                                        if (i > txt.Length - 1)
                                                        {
                                                            datatowrite += ' ';
                                                        }
                                                        else
                                                        {

                                                            if (i == txt.Length - 1)
                                                            {
                                                                datatowrite += ' ';
                                                            }
                                                            else if (i == txt.Length - 2)
                                                            {
                                                                datatowrite += ' ';
                                                            }
                                                            else if (i == txt.Length - 3)
                                                            {
                                                                datatowrite += ' ';
                                                            }
                                                            else
                                                            {
                                                                datatowrite += txt[i];
                                                            }
                                                        }
                                                    }
                                                    goto Write;
                                                }
                                                else
                                                {
                                                    goto Read;
                                                }

                                            Write:
                                                Byte[] data = Encoding.ASCII.GetBytes(datatowrite);

                                                fs.Write(data, 0, data.Length);
                                                fs.Close();
                                                Console.ForegroundColor = ConsoleColor.Green;
                                                response = "Your File was written";
                                                Console.ForegroundColor = ConsoleColor.White;
                                                break;
                                            }
                                            else
                                            {

                                                response = "Error writing file. Not open for writing.";
                                            }

                                        }
                                        else if (args[2] == "-read")
                                        {
                                            FileStream fs = (FileStream)Sys.FileSystem.VFS.VFSManager.GetFile("0:" + arg[0]).GetFileStream();
                                            if (fs.CanRead)
                                            {

                                                Byte[] data = new Byte[256];

                                                fs.Read(data, 0, data.Length);
                                                fs.Close();
                                                response = "Data written in file : \n" + Encoding.ASCII.GetString(data);
                                                break;
                                            }
                                            else
                                            {
                                                response = "Error reading file. Not open for reading.";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Console.Write("open : Invalid argument " + args[2] + ".\n");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    response = "Error writing file. Error Code : " + ex.ToString();
                                    break;
                                }

                                break;

                            case "help":
                                Console.WriteLine("Use arguments as follows : ");
                                Console.WriteLine("     Argument    Use");
                                Console.WriteLine("--------------------------------------------------------------------------------");
                                Console.WriteLine("     mk          Make File (write file name and location infront of it)         ");
                                Console.WriteLine("--------------------------------------------------------------------------------");
                                Console.WriteLine("     rm          Remove File (write file name and location infront of it)       ");
                                Console.WriteLine("--------------------------------------------------------------------------------");
                                Console.WriteLine("     mkdir       Make Directory (write directory name and location infront of   ");
                                Console.WriteLine("                 it)                                                            ");
                                Console.WriteLine("--------------------------------------------------------------------------------");
                                Console.WriteLine("     rmdir       Remove Directory (write directory name and location infront of ");
                                Console.WriteLine("                 it)                                                            ");
                                Console.WriteLine("--------------------------------------------------------------------------------");
                                Console.WriteLine("     open        Write to File (write file name and location infront of it)     ");
                                Console.WriteLine("                 has two another arguments '-read' to read and '-write' to wtite");
                                break;
            */

                case "ls":

                    if (!(Sys.FileSystem.VFS.VFSManager.FileExists("0:" + currentPath)) && (Sys.FileSystem.VFS.VFSManager.DirectoryExists("0:" + currentPath)))
                    {
                        var fileList = Directory.GetFiles(@"0:" + currentPath);
                        var directoryList = Directory.GetDirectories(@"0:" + currentPath);

                        foreach (var file in fileList)
                        {
                            Console.WriteLine(file);
                        }
                        foreach (var directory in directoryList)
                        {
                            Console.WriteLine(directory);
                        }
                    }
                    break;

                case "cd":
                    Console.Write("Write Path : ");
                    string path = Console.ReadLine();

                    currentPath = "\\" + path;
                    if (path == "\\")
                        currentPath = "\\";

                    break;

                case "pwd":
                    Console.WriteLine(currentPath);
                    break;

                case "exit":
                    exit = 1;
                    break;


                default:
                    response = "Unexpected argument : " + command;
                    break;
            }
            if (exit == 0)
                goto read;
            return response;
        }
    }
}
