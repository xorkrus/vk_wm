/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System.Drawing;
using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Components.GDI
{
    /// <summary>
    /// Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    [ComImport, Guid("327ABDAA-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IBitmapImage
    {
        uint GetSize(out Size size);
        uint GetPixelFormatID(out PixelFormatID pixelFormat);
        uint LockBits(ref Rectangle rect, uint flags, PixelFormatID pixelFormat, out BitmapData lockedBitmapData);
        uint UnlockBits(ref BitmapData lockedBitmapData);
        uint GetPalette();  // This is a place holder, note the lack of arguments
        uint SetPalette();  // This is a place holder, note the lack of arguments
    }
}