using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Notification.NfnsWithSoftKeys
{
  [StructLayout(LayoutKind.Sequential)]
	internal struct SHNOTIFICATIONDATA
	{
		// для верификации и версионности
		public int     cbStruct;
		// идентификатор для этой частичной верификации
		public int      dwID;
		// приоритет
		public SHNP		npPriority;
		/// <summary>
		/// Длительность появления бабблов ( в сеукндах)
		/// </summary>
		public int		csDuration;
		// Иконка для нотификации
		public IntPtr	hicon;
		/// <summary>
		/// Флаги, влияющие на поведение бабблов нотификации
		/// </summary>
		public SHNF	grfFlags;
		// Уникальный идентификатор для класса нотификаций
		public Guid		clsid;
		// Окно для возвращения компанд выбора, освобождения и т.д.
		public IntPtr	hwndSink;
		// HTML содержимое баббла
    [MarshalAs(UnmanagedType.LPTStr)]
    public string pszHTML;
		// Опциональный заголовой баббла
		[MarshalAs(UnmanagedType.LPTStr)]
    public string pszTitle;
		/// <summary>
		/// Параметр, определяемый пользователем
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
		/// Баббл, показываемый для задержки, потом исчезает
		/// </summary>
		INFORM = 0x1B1,
		/// <summary>
		/// Без баббла. Иконка, показываемая для задержки, потом исчезает
		/// </summary>
		ICONIC = 0,
	}

  // Маска обновлений нотификации
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
