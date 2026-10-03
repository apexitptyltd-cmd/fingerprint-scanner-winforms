using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sample
{
    public class BitmapFormat
    {
        // Define structures with explicit 1-byte alignment to match standard BMP file header structures
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPFILEHEADER
        {
            public ushort bfType;       // Must be 0x4D42 ("BM")
            public uint bfSize;         // Total size of the file in bytes
            public ushort bfReserved1;  // Reserved; must be 0
            public ushort bfReserved2;  // Reserved; must be 0
            public uint bfOffBits;      // Offset to the start of pixel data from header start
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;         // Size of this header structure (40 bytes)
            public int biWidth;         // Width of the image in pixels
            public int biHeight;        // Height of the image in pixels
            public ushort biPlanes;     // Number of color planes (must be 1)
            public ushort biBitCount;   // Number of bits per pixel (8 for grayscale)
            public uint biCompression;  // Compression type (0 for uncompressed RGB)
            public uint biSizeImage;    // Size of the raw image data payload padding (can be 0 for BI_RGB)
            public int biXPelsPerMeter; // Horizontal resolution
            public int biYPelsPerMeter; // Vertical resolution
            public uint biClrUsed;      // Number of colors in the color palette (256)
            public uint biClrImportant; // Number of important colors (256)
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MASK
        {
            public byte bluemask;       // Blue color channel weight component
            public byte greenmask;      // Green color channel weight component
            public byte redmask;        // Red color channel weight component
            public byte rgbReserved;    // Reserved padding element; must be 0
        }

        /// <summary>
        /// Converts structural context metadata objects directly into native memory byte payloads.
        /// </summary>
        public static byte[] StructToBytes(object structObj, int size)
        {
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, structPtr, false);
                Marshal.Copy(structPtr, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }
        }

        /// <summary>
        /// Flips the raw buffer vertically to correct inverted imaging artifacts introduced by fingerprint scanner drivers.
        /// </summary>
        public static void RotatePic(byte[] buffer, int width, int height, ref byte[] resBuf)
        {
            if (buffer == null || resBuf == null || buffer.Length < width * height || resBuf.Length < width * height)
                return;

            for (int row = 0; row < height; row++)
            {
                // Invert row index trajectory mappings: copy top rows from scanner buffer into the bottom rows of the result buffer
                int srcOffset = row * width;
                int dstOffset = (height - 1 - row) * width;
                Array.Copy(buffer, srcOffset, resBuf, dstOffset, width);
            }
        }

        /// <summary>
        /// Generates a perfectly formatted, 8-bit uncompressed grayscale BMP structure inside a MemoryStream.
        /// Fixes row padding alignment requirements (4-byte alignment boundaries) and prevents stream pointer layout crashes.
        /// Compatible with 10-finger enrollment sequence from both hands.
        /// </summary>
        public static void GetBitmap(byte[] buffer, int nWidth, int nHeight, ref MemoryStream ms)
        {
            if (buffer == null || ms == null) return;

            int ColorIndex = 0;
            ushort m_nBitCount = 8;
            int m_nColorTableEntries = 256;

            // Allocate exactly bounded array tracking workspace dimensions criteria context
            byte[] ResBuf = new byte[nWidth * nHeight];

            try
            {
                BITMAPFILEHEADER BmpHeader = new BITMAPFILEHEADER();
                BITMAPINFOHEADER BmpInfoHeader = new BITMAPINFOHEADER();
                MASK[] ColorMask = new MASK[m_nColorTableEntries];

                // CRITICAL FIX: Stride width allocation logic calculations rule (must be a multiple of 4 bytes)
                int w = (((nWidth + 3) / 4) * 4);

                // Initialize Image Info Header attributes bounds
                BmpInfoHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                BmpInfoHeader.biWidth = nWidth;
                BmpInfoHeader.biHeight = nHeight;
                BmpInfoHeader.biPlanes = 1;
                BmpInfoHeader.biBitCount = m_nBitCount;
                BmpInfoHeader.biCompression = 0; // BI_RGB Uncompressed
                BmpInfoHeader.biSizeImage = (uint)(w * nHeight);
                BmpInfoHeader.biXPelsPerMeter = 0;
                BmpInfoHeader.biYPelsPerMeter = 0;
                BmpInfoHeader.biClrUsed = (uint)m_nColorTableEntries;
                BmpInfoHeader.biClrImportant = (uint)m_nColorTableEntries;

                // Initialize File Structural Header attributes bounds
                BmpHeader.bfType = 0x4D42; // "BM" signatures validation marks
                BmpHeader.bfOffBits = (uint)(14 + Marshal.SizeOf(typeof(BITMAPINFOHEADER)) + (m_nColorTableEntries * 4));
                BmpHeader.bfSize = (uint)(BmpHeader.bfOffBits + BmpInfoHeader.biSizeImage);
                BmpHeader.bfReserved1 = 0;
                BmpHeader.bfReserved2 = 0;

                // Commit structures out directly into memory pipeline components
                byte[] fileHeaderBytes = StructToBytes(BmpHeader, 14);
                byte[] infoHeaderBytes = StructToBytes(BmpInfoHeader, Marshal.SizeOf(typeof(BITMAPINFOHEADER)));

                ms.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                ms.Write(infoHeaderBytes, 0, infoHeaderBytes.Length);

                // Write the 256-color Grayscale Palette (Color Table entries)
                for (ColorIndex = 0; ColorIndex < m_nColorTableEntries; ColorIndex++)
                {
                    ColorMask[ColorIndex].redmask = (byte)ColorIndex;
                    ColorMask[ColorIndex].greenmask = (byte)ColorIndex;
                    ColorMask[ColorIndex].bluemask = (byte)ColorIndex;
                    ColorMask[ColorIndex].rgbReserved = 0;

                    byte[] maskBytes = StructToBytes(ColorMask[ColorIndex], Marshal.SizeOf(typeof(MASK)));
                    ms.Write(maskBytes, 0, maskBytes.Length);
                }

                // Invert / flip image buffer vertically to correct upside-down presentation patterns
                RotatePic(buffer, nWidth, nHeight, ref ResBuf);

                // Write scanlines to memory buffer stream, appending alignment row padding bytes where required
                int paddingSize = w - nWidth;
                byte[] paddingBuffer = paddingSize > 0 ? new byte[paddingSize] : null;

                for (int i = 0; i < nHeight; i++)
                {
                    // Write valid active scanner pixels line
                    ms.Write(ResBuf, i * nWidth, nWidth);

                    // CRITICAL FIX: Append true empty alignment buffer blocks, NOT raw target image payload data array offsets
                    if (paddingSize > 0)
                    {
                        ms.Write(paddingBuffer, 0, paddingSize);
                    }
                }

                // CRITICAL FIX: Reset stream memory layout pointer position back to index 0 so PictureBox drawing pipelines can consume data stream cleanly
                ms.Position = 0;
            }
            catch (Exception)
            {
                // Fail silently or manage internal logging tracing handles safely locally here
                throw;
            }
        }
    }
}
