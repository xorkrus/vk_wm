using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Notification.NfnsWithSoftKeys
{
  [StructLayout(LayoutKind.Sequential)]
	internal struct SHNOTIFICATIONDATA
	{
		// ��� ����������� � ������������
		public int     cbStruct;
		// ������������� ��� ���� ��������� �����������
		public int      dwID;
		// ���������
		public SHNP		npPriority;
		/// <summary>
		/// ������������ ��������� ������� ( � ��������)
		/// </summary>
		public int		csDuration;
		// ������ ��� �����������
		public IntPtr	hicon;
		/// <summary>
		/// �����, �������� �� ��������� ������� �����������
		/// </summary>
		public SHNF	grfFlags;
		// ���������� ������������� ��� ������ �����������
		public Guid		clsid;
		// ���� ��� ����������� ������� ������, ������������ � �.�.
		public IntPtr	hwndSink;
		// HTML ���������� ������
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pszHTML;
		// ������������ ��������� ������
		[MarshalAs(UnmanagedType.LPTStr)]
    public string pszTitle;
		/// <summary>
		/// ��������, ������������ �������������
		/// </summary>
		public int		lParam;

    public SOFTKEYNOTIFY leftSoftKey;
    public SOFTKEYNOTIFY rightSoftKey;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pszTodaySK;
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pszTodayExec;
	}

  [StructLayout(LayoutKind.Sequential)]
  struct SOFTKEYCMD
  {
    public UInt32 wpCmd;
    public UInt32 grfFlags;

  }

  [StructLayout(LayoutKind.Sequential)]
  struct SOFTKEYNOTIFY
  {
    [MarshalAs(UnmanagedType.LPTStr)]
    public String pszTitle;
    public SOFTKEYCMD skc;
  }

	internal enum SHNP :int
	{
		/// <summary>
		/// �����, ������������ ��� ��������, ����� ��������
		/// </summary>
		INFORM = 0x1B1,
		/// <summary>
		/// ��� ������. ������, ������������ ��� ��������, ����� ��������
		/// </summary>
		ICONIC = 0,
	}

  // ����� ���������� �����������
  internal enum SHNUM : int
  {
    PRIORITY    = 0x0001,
    DURATION    = 0x0002,
    ICON        = 0x0004,
    HTML        = 0x0008,
    TITLE       = 0x0010,
    SOFTKEYS    = 0x0020,
    TODAYKEY    = 0x0040,
    TODAYEXEC   = 0x0080,
    SOFTKEYCMDS = 0x0100,
    FLAGS       = 0x0200
  }

	/// <summary>
	/// Flags that affect the display behaviour of the Notification
	/// </summary>
	internal enum SHNF : int
	{
		/// <summary>
		/// For SHNP_INFORM priority and above, don't display the notification bubble when it's initially added;
		/// the icon will display for the duration then it will go straight into the tray.
		/// The user can view the icon / see the bubble by opening the tray.
		/// </summary>
		STRAIGHTTOTRAY  = 0x00000001,
		/// <summary>
		/// Critical information - highlights the border and title of the bubble.
		/// </summary>
		CRITICAL        = 0x00000002,
		/// <summary>
		/// Force the message (bubble) to display even if settings says not to.
		/// </summary>
		FORCEMESSAGE    = 0x00000008,
		/// <summary>
		/// Force the display to turn on for notification. Added for Windows Mobile 2003.
		/// </summary>
		DISPLAYON       = 0x00000010,
		/// <summary>
		/// Force the notification to be silent and not vibrate, regardless of Settings. Added for Windows Mobile 2003.
		/// </summary>
		SILENT          = 0x00000020,
    
    // Draw the current time with the title
    TITLETIME   =    0x00000080,

    // A notification with "stack" support
    SPINNERS   =     0x00000100,

    // RE-play physical alerts on an update
    SHNF_ALERTONUPDATE  = 0x00000200
	}

	internal struct NMSHN
	{
		public IntPtr hwndFrom; 
		public int idFrom; 
		public SHNN code; 
		public int lParam;
		public int dwReturn;
		public int union1;
		public int union2;
  }
}
