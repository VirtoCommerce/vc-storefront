using System;
using System.IO;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream fromStream, Stream toStream)
        {
            if (fromStream == null)
            {
                throw new ArgumentNullException("fromStream");
            }
            if (toStream == null)
            {
                throw new ArgumentNullException("toStream");
            }

            var bytes = new byte[8092];
            int dataRead;
            while ((dataRead = fromStream.Read(bytes, 0, bytes.Length)) > 0)
            {
                toStream.Write(bytes, 0, dataRead);
            }
        }

        public static byte[] ReadFully(this Stream stream)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string ReadToString(this Stream stream)
        {
            // convert stream to string
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<string> ReadToStringAsync(this Stream stream)
        {
            // convert stream to string
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
