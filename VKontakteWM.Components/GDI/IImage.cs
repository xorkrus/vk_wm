/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Galssoft.VKontakteWM.Components.GDI
{
    /// <summary>
    /// Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    [ComImport, Guid("327ABDA9-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IImage
    {
        uint GetPhysicalDimension(out Size size);
        uint GetImageInfo(out ImageInfo info);
        uint SetImageFlags(uint flags);
        uint Draw(IntPtr hdc, ref Rectangle dstRect, IntPtr NULL); // "Correct" declaration: uint Draw(IntPtr hdc, ref Rectangle dstRect, ref Rectangle srcRect);
        uint PushIntoSink();    // This is a place holder, note the lack of arguments
        uint GetThumbnail(uint thumbWidth, uint thumbHeight, out IImage thumbImage);
    }
}