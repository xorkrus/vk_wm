using System;
using System.IO;
using System.Net;
using Galssoft.VKontakteWM.Components.Common.SystemHelpers;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;

namespace Galssoft.VKontakteWM.Components.Server
{
    /// <summary>
    /// Класс для использования архивированных данных.
    /// </summary>
    public class CompressedWebResponse : WebResponse
    {
        private readonly HttpWebResponse _response;

        internal CompressedWebResponse(WebResponse response)
        {
            //((HttpWebRequest)request).UserAgent = SystemConfiguration.UserAgentString;
            _response = (HttpWebResponse)response;
        }

        // select the right decompression stream  
        public override Stream GetResponseStream()
        {
            try
            {
                Stream responseStream = _response.GetResponseStream();
                Stream compressedStream = null;
                if (_response.ContentEncoding == "gzip")
                {
                    compressedStream = new
                      GZipInputStream(responseStream);
                }
                else if (_response.ContentEncoding == "deflate")
                {
                    compressedStream = new
                      ZipInputStream(responseStream);
                }

                if (compressedStream == null)
                    compressedStream = responseStream;

                if (compressedStream != null)
                {
                    // Decompress
                    MemoryStream decompressedStream = new MemoryStream();
                    int totalSize = 0;
                    int size = 2048;
                    byte[] writeData = new byte[2048];
                    while (true)
                    {
                        size = compressedStream.Read(writeData, 0, size);
                        totalSize += size;
                        if (size > 0)
                            decompressedStream.Write(writeData, 0, size);
                        else
                            break;
                    }

                    if (compressedStream != responseStream)
                        responseStream.Close();
                    compressedStream.Close();

                    decompressedStream.Seek(0, SeekOrigin.Begin);

                    using (MemoryStream logstream = new MemoryStream(decompressedStream.GetBuffer()))
                    using (StreamReader sr = new StreamReader(logstream))
                    {
                        DebugHelper.WriteLogEntry("ResponseStream: " + sr.ReadToEnd());
                    }

                    decompressedStream.Seek(0, SeekOrigin.Begin);
                    return decompressedStream;
                }
                return compressedStream;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLogEntry(ex, "GZIP error");
                return null;
            }
        }

        public override long ContentLength
        {
            get { return _response.ContentLength; }
        }
        public override string ContentType
        {
            get { return _response.ContentType; }
        }
        public override WebHeaderCollection Headers
        {
            get { return _response.Headers; }
        }
        public override System.Uri ResponseUri
        {
            get { return _response.ResponseUri; }
        }

        public override void Close()
        {
            _response.Close();
        }
    }
}
