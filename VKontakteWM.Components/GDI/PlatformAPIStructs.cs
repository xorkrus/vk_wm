/*	 This file was created by GALS Soft	company
 *	 http://www.galssoft.com
 *	 
 *	 Project name is Odnoklassniki Mobile
 *	 Platform is .Net Compact Framework 2.0 for Windows Mobile 5.0+ 
 * 
 *	 Copyright (c) 2009-2010 GALS Soft
 */

using System;

namespace Galssoft.VKontakteWM.Components.GDI
{
    /// <summary>
    /// Pulled from gdipluspixelformats.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum PixelFormatID : int
    {
        PixelFormatIndexed = 0x00010000, // Indexes into a palette
        PixelFormatGDI = 0x00020000, // Is a GDI-supported format
        PixelFormatAlpha = 0x00040000, // Has an alpha component
        PixelFormatPAlpha = 0x00080000, // Pre-multiplied alpha
        PixelFormatExtended = 0x00100000, // Extended color 16 bits/channel
        PixelFormatCanonical = 0x00200000,

        PixelFormatUndefined = 0,
        PixelFormatDontCare = 0,

        PixelFormat1bppIndexed = (1 | (1 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat4bppIndexed = (2 | (4 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat8bppIndexed = (3 | (8 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat16bppRGB555 = (5 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppRGB565 = (6 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppARGB1555 = (7 | (16 << 8) | PixelFormatAlpha | PixelFormatGDI),
        PixelFormat24bppRGB = (8 | (24 << 8) | PixelFormatGDI),
        PixelFormat32bppRGB = (9 | (32 << 8) | PixelFormatGDI),
        PixelFormat32bppARGB = (10 | (32 << 8) | PixelFormatAlpha | PixelFormatGDI | PixelFormatCanonical),
        PixelFormat32bppPARGB = (11 | (32 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatGDI),
        PixelFormat48bppRGB = (12 | (48 << 8) | PixelFormatExtended),
        PixelFormat64bppARGB = (13 | (64 << 8) | PixelFormatAlpha | PixelFormatCanonical | PixelFormatExtended),
        PixelFormat64bppPARGB = (14 | (64 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatExtended),
        PixelFormatMax = 15
    }

    /// <summary>
    /// Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum BufferDisposalFlag : int
    {
        BufferDisposalFlagNone,
        BufferDisposalFlagGlobalFree,
        BufferDisposalFlagCoTaskMemFree,
        BufferDisposalFlagUnmapView
    }

    /// <summary>
    /// Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum InterpolationHint : int
    {
        InterpolationHintDefault,
        InterpolationHintNearestNeighbor,
        InterpolationHintBilinear,
        InterpolationHintAveraging,
        InterpolationHintBicubic
    }

    /// <summary>
    /// Pulled from gdiplusimaging.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public struct BitmapData
    {
        public uint Width;
        public uint Height;
        public int Stride;
        public PixelFormatID PixelFormat;
        public IntPtr Scan0;
        public IntPtr Reserved;
    }

    /// <summary>
    /// Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public struct ImageInfo
    {
        public uint GuidPart1;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public uint GuidPart2;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public uint GuidPart3;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public uint GuidPart4;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public PixelFormatID pixelFormat;
        public uint Width;
        public uint Height;
        public uint TileWidth;
        public uint TileHeight;
        public double Xdpi;
        public double Ydpi;
        public uint Flags;
    }

    /// <summary>
    /// Pulled from wingdi.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    /// <summary>
    /// Pulled from wingdi.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum BlendOperation : byte
    {
        AC_SRC_OVER = 0x00
    }

    /// <summary>
    /// Pulled from wingdi.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum BlendFlags : byte
    {
        Zero = 0x00
    }

    /// <summary>
    /// Pulled from wingdi.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum SourceConstantAlpha : byte
    {
        Transparent = 0x00,
        Opaque = 0xFF
    }

    /// <summary>
    /// Pulled from wingdi.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum AlphaFormat : byte
    {
        AC_SRC_ALPHA = 0x01
    }

    /// <summary>
    /// Pulled from wingdi.h in the Windows Mobile 5.0 Pocket PC SDK
    /// </summary>
    public enum TernaryRasterOperations : uint
    {
        /// <summary>dest = source</summary>
        SRCCOPY = 0x00CC0020,
        /// <summary>dest = source OR dest</summary>
        SRCPAINT = 0x00EE0086,
        /// <summary>dest = source AND dest</summary>
        SRCAND = 0x008800C6,
        /// <summary>dest = source XOR dest</summary>
        SRCINVERT = 0x00660046,
        /// <summary>dest = source AND (NOT dest)</summary>
        SRCERASE = 0x00440328,
        /// <summary>dest = (NOT source)</summary>
        NOTSRCCOPY = 0x00330008,
        /// <summary>dest = (NOT src) AND (NOT dest)</summary>
        NOTSRCERASE = 0x001100A6,
        /// <summary>dest = (source AND pattern)</summary>
        MERGECOPY = 0x00C000CA,
        /// <summary>dest = (NOT source) OR dest</summary>
        MERGEPAINT = 0x00BB0226,
        /// <summary>dest = pattern</summary>
        PATCOPY = 0x00F00021,
        /// <summary>dest = DPSnoo</summary>
        PATPAINT = 0x00FB0A09,
        /// <summary>dest = pattern XOR dest</summary>
        PATINVERT = 0x005A0049,
        /// <summary>dest = (NOT dest)</summary>
        DSTINVERT = 0x00550009,
        /// <summary>dest = BLACK</summary>
        BLACKNESS = 0x00000042,
        /// <summary>dest = WHITE</summary>
        WHITENESS = 0x00FF0062
    }
}