using CodeInject.MemoryTools;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpNativeDLL
{
    public class EntryPoint
    {
        private const uint DLL_PROCESS_DETACH = 0,
                           DLL_PROCESS_ATTACH = 1,
                           DLL_THREAD_ATTACH = 2,
                           DLL_THREAD_DETACH = 3;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [UnmanagedCallersOnly(EntryPoint = "DllMain", CallConvs = new[] { typeof(CallConvStdcall) })]
        public static bool DllMain(IntPtr hModule, uint ul_reason_for_call, IntPtr lpReserved)
        {
            switch (ul_reason_for_call)
            {
                case DLL_PROCESS_ATTACH:

                    //AllocConsole();
                    Console.WriteLine("RUN");
                    while (true)
                    {
                        var server = new NamedPipeServerStream($"Luigi{System.Diagnostics.Process.GetCurrentProcess().Id}", PipeDirection.InOut);
                        try
                        {
                            Console.WriteLine("Waiting for client...");
                            server.WaitForConnection();
                            Console.WriteLine("Client connected!");
                            using (var reader = new StreamReader(server))
                            using (var writer = new StreamWriter(server))
                            {
                                writer.WriteLine(GameHackFunc.Game.ClientData.ToString());
                                writer.Flush();

                                Console.WriteLine("Send message,");
                                while (true)
                                {
                                    string receivedData = reader.ReadLine();

                                    Console.WriteLine("Recived message,");
                                    if (receivedData.Contains("GETITEMDETAILS"))
                                    {
                                        string[] commandData = receivedData.Split(';');

                                        long result = GameHackFunc.Game.ClientData.GetInventoryItemDetails(long.Parse(commandData[1]));
                                        Console.WriteLine($"Process items {result.ToString("X")}");
                                        writer.WriteLine($"ITEMDETAILS;{result}");
                                        writer.Flush();
                                    }
                                    else if (receivedData.Contains("ATTACKTARGET"))
                                    {
                                        string[] commandData = receivedData.Split(';');
                                        GameHackFunc.Game.Actions.Attack(int.Parse(commandData[1]));
                                    }
                                    else if (receivedData.Contains("CASTSPELLONTARGET"))
                                    {
                                        string[] commandData = receivedData.Split(';');
                                        GameHackFunc.Game.Actions.CastSpell(int.Parse(commandData[1]), int.Parse(commandData[2]));
                                    }
                                    else if (receivedData.Contains("CASTBUFF"))
                                    {
                                        string[] commandData = receivedData.Split(';');
                                        GameHackFunc.Game.Actions.CastSpell(int.Parse(commandData[1]));
                                    }

                                    else if (receivedData.Contains("PICKUP"))
                                    {
                                        string[] commandData = receivedData.Split(';');
                                        GameHackFunc.Game.Actions.PickUp(long.Parse(commandData[1]), int.Parse(commandData[2]));
                                    }
                                    else if (receivedData.Contains("USEITEM"))
                                    {
                                        string[] commandData = receivedData.Split(';');
                                        GameHackFunc.Game.Actions.ItemUse(long.Parse(commandData[1]));
                                    }
                                    else if (receivedData.Contains("MOVETO"))
                                    {
                                        string[] commandData = receivedData.Split(';');
                                        GameHackFunc.Game.Actions.MoveToPoint(new System.Numerics.Vector2(float.Parse(commandData[1]), float.Parse(commandData[2])));
                                    }
                                    else if (receivedData.Contains("REPAIR"))
                                    {
                                        string[] commandData = receivedData.Split(';');
                                        GameHackFunc.Game.Actions.RepairItem(int.Parse(commandData[1]), int.Parse(commandData[2]));
                                    }
                                }
                            }
                        }
                        catch (IOException ioEx)
                        {
                            if (ioEx.Message.Contains("Pipe is broken"))
                            {
                                Console.WriteLine("Pipe is broken! Próba ponownego połączenia...");
                            };
                        }
                    }

                    break;
                default:
                    break;
            }
            return true;
        }
    }
}