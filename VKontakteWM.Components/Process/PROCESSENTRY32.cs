using System;
using System.Text;

namespace Galssoft.VKontakteWM.Components.Process
{
    class PROCESSENTRY32
    {
        // constants for structure definition
        private const int SizeOffset = 0;
        private const int UsageOffset = 4;
        private const int ProcessIdOffset = 8;
        private const int DefaultHeapIdOffset = 12;
        private const int ModuleIdOffset = 16;
        private const int ThreadsOffset = 20;
        private const int ParentProcessIdOffset = 24;
        private const int PriClassBaseOffset = 28;
        private const int DwFlagsOffset = 32;
        private const int ExeFileOffset = 36;
        private const int MemoryBaseOffset = 556;
        private const int AccessKeyOffset = 560;
        private const int Size = 564; // the whole size of the structure
        private const int MaxPath = 260;

        // data members
        public uint DwSize;
        public uint CntUsage;
        public uint Th32ProcessId;
        public uint Th32DefaultHeapId;
        public uint Th32ModuleId;
        public uint CntThreads;
        public uint Th32ParentProcessId;
        public int PcPriClassBase;
        public uint DwFlags;
        public string SzExeFile;
        public uint Th32MemoryBase;
        public uint Th32AccessKey;

        // create a PROCESSENTRY instance based on a byte array      
        public PROCESSENTRY32(byte[] aData)
        {
            DwSize = GetUInt(aData, SizeOffset);
            CntUsage = GetUInt(aData, UsageOffset);
            Th32ProcessId = GetUInt(aData, ProcessIdOffset);
            Th32DefaultHeapId = GetUInt(aData, DefaultHeapIdOffset);
            Th32ModuleId = GetUInt(aData, ModuleIdOffset);
            CntThreads = GetUInt(aData, ThreadsOffset);
            Th32ParentProcessId = GetUInt(aData, ParentProcessIdOffset);
            PcPriClassBase = (int) GetUInt(aData, PriClassBaseOffset);
            DwFlags = GetUInt(aData, DwFlagsOffset);
            SzExeFile = GetString(aData, ExeFileOffset, MaxPath);
            Th32MemoryBase = GetUInt(aData, MemoryBaseOffset);
            Th32AccessKey = GetUInt(aData, AccessKeyOffset);
        }

        public PROCESSENTRY32()
        {
            
        }

        public string Name
        {
            get
            {
                return SzExeFile.Substring(0, SzExeFile.IndexOf('\0'));
            }
        }

        public ulong Pid
        {
            get
            {
                return Th32ProcessId;
            }
        }

        public ulong BaseAddress
        {
            get
            {
                return Th32MemoryBase;
            }
        }

        public ulong ThreadCount
        {
            get
            {
                return CntThreads;
            }
        }

        // utility:  get a unicode string from the byte array
        private static string GetString(byte[] aData, int offset, int length)
        {
            String sReturn = Encoding.Unicode.GetString(aData, offset, length);
            return sReturn;
        }

        // utility:  set a unicode string in the byte array
        private static void SetString(byte[] aData, int offset, string value)
        {
            byte[] arr = Encoding.ASCII.GetBytes(value);
            Buffer.BlockCopy(arr, 0, aData, offset, arr.Length);
        }

        public byte[] ToByteArray()
        {
            byte[] aData = new byte[Size];
            //set the Size member
            SetUInt(aData, SizeOffset, Size);
            return aData;
        }

        // utility:  get a uint from the byte array
        private static uint GetUInt(byte[] aData, int offset)
        {
            return BitConverter.ToUInt32(aData, offset);
        }

        // utility:  set a uint into the byte array
        private static void SetUInt(byte[] aData, int offset, int value)
        {
            byte[] buint = BitConverter.GetBytes(value);
            Buffer.BlockCopy(buint, 0, aData, offset, buint.Length);
        }
        // utility:  get a ushort from the byte array
        private static ushort GetUShort(byte[] aData, int offset)
        {
            return BitConverter.ToUInt16(aData, offset);
        }

        // utility:  set a ushort int the byte array
        private static void SetUShort(byte[] aData, int offset, int value)
        {
            byte[] bushort = BitConverter.GetBytes((short)value);
            Buffer.BlockCopy(bushort, 0, aData, offset, bushort.Length);
        }
    }
}
