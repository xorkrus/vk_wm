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
    public static class ImagingFactory
    {
        private static IImagingFactory factory;

        public static IImagingFactory GetImaging()
        {
            if (factory == null)
            {
                factory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
            }

            return factory;
        }

    }
}