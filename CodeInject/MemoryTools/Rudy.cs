using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Winebotv2.MemoryTools
{
    /// <summary>
    /// Game Memory reading class
    /// </summary>
    public class Rudy
    {

        public static Rudy Instance { get { return _instance; } }


        private static Rudy _instance = new Rudy();
        private IntPtr processHandle;

        private Rudy() 
        {
     
        }

        public void OpenProcess(int id)
        {
            OpenProces(id);
        }

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            UIntPtr lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            out int lpNumberOfBytesRead);

        // Definicja OpenProcess z Windows API (aby otworzyć proces z odpowiednimi uprawnieniami)
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            int processAccess,
            bool bInheritHandle,
            int processId);

        public const int PROCESS_VM_READ = 0x0010;



        public string ReadString(UIntPtr addres)
        {
            UIntPtr baseAddress = addres;

            int maxStringLengthBytes = 64;
            byte[] buffer = new byte[maxStringLengthBytes];

            int bytesRead = 0;


            bool success = ReadProcessMemory(processHandle, baseAddress, buffer, buffer.Length, out bytesRead);


            if (success && bytesRead > 0)
            {
                int stringLength = Array.IndexOf(buffer, (byte)0);

                if (stringLength < 0) stringLength = maxStringLengthBytes;


                string resultString = Encoding.ASCII.GetString(buffer, 0, stringLength);
                return resultString;
            }


            return "Could not read data from process";
        }

        public int ReadInt(UIntPtr address)
        {
            byte[] buffer = new byte[4];
            int bytesRead = 0;

            bool success = ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);

            if (success && bytesRead == buffer.Length)
            {
                return BitConverter.ToInt32(buffer, 0);
            }

            return 0;
            throw new Exception("Could not read int value from process");
        }

        public ushort ReadUShort(UIntPtr address)
        {
            byte[] buffer = new byte[2];
            int bytesRead = 0;

            bool success = ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);

            if (success && bytesRead == buffer.Length)
            {
                return BitConverter.ToUInt16(buffer, 0);
            }

            return 0;
            throw new Exception("Could not read ushort value from process");
        }

        public float ReadFloat(UIntPtr address)
        {
            byte[] buffer = new byte[4];
            int bytesRead = 0;

            bool success = ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);

            if (success && bytesRead == buffer.Length)
            {
                return BitConverter.ToSingle(buffer, 0);
            }

            return 0.0f;
            //throw new Exception("Could not read float value from process");
        }

        public long ReadLong(UIntPtr address)
        {
            byte[] buffer = new byte[8];
            int bytesRead = 0;

            bool success = ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);

            if (success && bytesRead == buffer.Length)
            {
                return BitConverter.ToInt64(buffer, 0);
            }

            return 0;
            throw new Exception("Could not read long value from process");
        }

        public ulong ReadULong(UIntPtr address)
        {
            if(address.ToUInt64()==0x0) return 0;

            byte[] buffer = new byte[8];
            int bytesRead = 0;

            bool success = ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);

            if (success && bytesRead == buffer.Length)
            {
                return BitConverter.ToUInt64(buffer, 0);
            }

            return 0;
            throw new Exception("Could not read long value from process");
        }


        public void OpenProces(int pid)
        {
            Process process = Process.GetProcessById(pid);

            processHandle = OpenProcess(PROCESS_VM_READ, false, process.Id);
        }

        public void OpenProces(string processName)
        {
            Process process = Process.GetProcessesByName(processName)[0];

            processHandle = OpenProcess(PROCESS_VM_READ, false, process.Id);
        }
    }
}
