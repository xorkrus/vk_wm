using System.Runtime.InteropServices;
using System.Text;

namespace Galssoft.VKontakteWM.Components.Common.SystemHelpers
{
    internal class MemoryStatus
    {
        // Methods
        public static string GetMemoryInfo()
        {
            uint lpdwStorePages = 0;
            uint lpdwRamPages = 0;
            uint lpdwPageSize = 0;
            GetSystemMemoryDivision(ref lpdwStorePages, ref lpdwRamPages, ref lpdwPageSize);
            MEMORYSTATUS lpBuffer = new MEMORYSTATUS();
            GlobalMemoryStatus(ref lpBuffer);
            StringBuilder builder = new StringBuilder();
            builder.Append("\r\nMemory Load: " + lpBuffer.dwMemoryLoad.ToString() + "\r\n");
            builder.Append("Total Physical Memory: " + lpBuffer.dwTotalPhys.ToString() + "\r\n");
            builder.Append("Avail Physical Memory: " + lpBuffer.dwAvailPhys.ToString() + "\r\n");
            builder.Append("Total Virtual Memory: " + lpBuffer.dwTotalVirtual.ToString() + "\r\n");
            builder.Append("Avail Virtual Memory: " + lpBuffer.dwAvailVirtual.ToString());
            return builder.ToString();
        }

        [DllImport("CoreDll.dll")]
        public static extern int GetSystemMemoryDivision(ref uint lpdwStorePages, ref uint lpdwRamPages, ref uint lpdwPageSize);
        [DllImport("CoreDll.dll")]
        public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUS
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public uint dwTotalPhys;
            public uint dwAvailPhys;
            public uint dwTotalPageFile;
            public uint dwAvailPageFile;
            public uint dwTotalVirtual;
            public uint dwAvailVirtual;
        }
    }
}