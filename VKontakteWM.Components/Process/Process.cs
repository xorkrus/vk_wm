using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Components.Process
{
    public class Process
    {
        #region P/Invoke

        private const int TH32CS_SNAPPROCESS = 0x00000002;
        private const int TH32CS_SNAPNOHEAPS = 0x40000000;
        [DllImport("toolhelp.dll")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processid);
        [DllImport("toolhelp.dll")]
        public static extern int CloseToolhelp32Snapshot(IntPtr handle);
        [DllImport("toolhelp.dll")]
        public static extern int Process32First(IntPtr handle, byte[] pe);
        [DllImport("toolhelp.dll")]
        public static extern int Process32Next(IntPtr handle, byte[] pe);
        [DllImport("coredll.dll")]
        private static extern IntPtr OpenProcess(int flags, bool fInherit, int PID);
        private const int PROCESS_TERMINATE = 1;
        [DllImport("coredll.dll")]
        private static extern bool TerminateProcess(IntPtr hProcess, uint ExitCode);
        [DllImport("coredll.dll")]
        private static extern bool CloseHandle(IntPtr handle);
        private const int INVALID_HANDLE_VALUE = -1;

        #endregion

        private readonly string _processName;
        private readonly IntPtr _handle;
        private readonly int _threadCount;
        private readonly int _baseAddress;

        //default constructor
        public Process()
        {

        }

        //private helper constructor
        private Process(IntPtr id, string procname, int threadcount, int baseaddress)
        {
            _handle = id;
            _processName = procname;
            _threadCount = threadcount;
            _baseAddress = baseaddress;
        }

        public static Process[] GetProcesses()
        {
            //temp ArrayList
            ArrayList procList = new ArrayList();

            IntPtr handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS | TH32CS_SNAPNOHEAPS, 0);

            if ((int)handle > 0)
            {
                try
                {
                    PROCESSENTRY32 peCurrent;
                    PROCESSENTRY32 pe32 = new PROCESSENTRY32();
                    //Get byte array to pass to the API calls
                    byte[] peBytes = pe32.ToByteArray();
                    //Get the first process
                    int retval = Process32First(handle, peBytes);
                    while (retval == 1)
                    {
                        //Convert bytes to the class
                        peCurrent = new PROCESSENTRY32(peBytes);
                        //New instance of the Process class
                        Process proc = new Process(new IntPtr((int)peCurrent.Pid),
                                       peCurrent.Name, (int)peCurrent.ThreadCount,
                                       (int)peCurrent.BaseAddress);

                        procList.Add(proc);

                        retval = Process32Next(handle, peBytes);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception: " + ex.Message);
                }
                //Close handle
                CloseToolhelp32Snapshot(handle);

                return (Process[])procList.ToArray(typeof(Process));

            }
            else
            {
                throw new Exception("Unable to create snapshot");
            }

        }

        public override string ToString()
        {
            return _processName;
        }

        public int BaseAddress
        {
            get
            {
                return _baseAddress;
            }
        }

        public int ThreadCount
        {
            get
            {
                return _threadCount;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }

        public string ProcessName
        {
            get
            {
                return _processName;
            }
        }

        public int BaseAddess
        {
            get
            {
                return _baseAddress;
            }
        }
    }
}
