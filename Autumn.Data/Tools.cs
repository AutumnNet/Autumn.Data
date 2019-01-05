using System.Data;
using System.IO;

namespace Autumn.Data
{
    public static class Tools
    {
        public static byte[] GetBlobBytes(this IDataReader reader, int i)
        {
            const int bufferSize = 256; // Size of the BLOB buffer.
            var outByte = new byte[bufferSize];    // The BLOB byte[] buffer to be filled by GetBytes.
            long startIndex = 0; 

            
            using(var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                var readValue = reader.GetBytes(i, startIndex, outByte, 0, bufferSize);                              // The bytes returned from GetBytes.
                while (readValue == bufferSize)
                {
                    bw.Write(outByte);
                    bw.Flush();
                    startIndex += bufferSize;
                    readValue = reader.GetBytes(i, startIndex, outByte, 0, bufferSize);
                }

                bw.Write(outByte, 0, (int) readValue);
                bw.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }
    }
}