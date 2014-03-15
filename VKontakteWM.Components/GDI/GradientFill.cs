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
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;

namespace Galssoft.VKontakteWM.Components.GDI
{
    /// <summary>
    /// The direction to the GradientFill will follow
    /// </summary>
    public enum FillDirection
    {
        // The fill goes horizontally
        LeftToRight = Win32.GRADIENT_FILL_RECT_H,
        // The fill goes vertically
        TopToBottom = Win32.GRADIENT_FILL_RECT_V
    }

    public sealed class GradientFill
    {
        public static bool Fill(
            Graphics gr,
            Rectangle rc,
            Color startColor, Color endColor,
            FillDirection fillDir)
        {
            // Get the hDC from the Graphics object.
            IntPtr hdc = gr.GetHdc();
            bool b = Fill(hdc, rc, startColor, endColor, fillDir);
            // Release the hDC from the Graphics object.
            gr.ReleaseHdc(hdc);
            return b;
        }

        /// <summary>
        /// This method wraps the PInvoke to GradientFill
        /// </summary>
        /// <param name="gr">The Graphics object we are filling</param>
        /// <param name="rc">The rectangle to fill</param>
        /// <param name="startColor">The starting color for the fill</param>
        /// <param name="endColor">The ending color for the fill</param>
        /// <param name="fillDir">The direction to fill</param>
        /// <returns>Returns true if the call to GradientFill succeeded; false otherwise.</returns>
        public static bool Fill(
            IntPtr hdc,
            Rectangle rc,
            Color startColor, Color endColor,
            FillDirection fillDir)
        {

            // Initialize the data to be used in the call to GradientFill.
            Win32.TRIVERTEX[] tva = new Win32.TRIVERTEX[2];
            tva[0] = new Win32.TRIVERTEX(rc.X, rc.Y, startColor);
            tva[1] = new Win32.TRIVERTEX(rc.Right, rc.Bottom, endColor);
            Win32.GRADIENT_RECT[] gra = new Win32.GRADIENT_RECT[] {
                                                                      new Win32.GRADIENT_RECT(0, 1)};


            // PInvoke to GradientFill.
            bool b = Win32.GradientFill(
                hdc,
                tva,
                (uint)tva.Length,
                gra,
                (uint)gra.Length,
                (uint)fillDir);

            System.Diagnostics.Debug.Assert(b, string.Format(
                                                   "GradientFill failed: {0}",
                                                   System.Runtime.InteropServices.Marshal.GetLastWin32Error()));

            return b;
        }


    }
}