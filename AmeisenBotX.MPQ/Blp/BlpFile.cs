using System.Drawing;
using System.Text;

namespace AmeisenBotX.MPQ.Blp
{
    public struct ARGBColor8
    {
        public byte red;
        public byte green;
        public byte blue;
        public byte alpha;

        public ARGBColor8(int r, int g, int b)
        {
            red = (byte)r;
            green = (byte)g;
            blue = (byte)b;
            alpha = 255;
        }

        public ARGBColor8(byte r, byte g, byte b)
        {
            red = r;
            green = g;
            blue = b;
            alpha = 255;
        }

        public ARGBColor8(int a, int r, int g, int b)
        {
            red = (byte)r;
            green = (byte)g;
            blue = (byte)b;
            alpha = (byte)a;
        }

        public ARGBColor8(byte a, byte r, byte g, byte b)
        {
            red = r;
            green = g;
            blue = b;
            alpha = a;
        }
        public static void convertToBGRA(ref byte[] pixel)
        {
            byte tmp = 0;

            for (int i = 0; i < pixel.Length; i += 4)
            {
                tmp = pixel[i];
                pixel[i] = pixel[i + 2];
                pixel[i + 2] = tmp;
            }
        }
    }

    public class BlpFile : IDisposable
    {
        uint type; // compression: 0 = JPEG Compression, 1 = Uncompressed or DirectX Compression
        byte encoding; // 1 = Uncompressed, 2 = DirectX Compressed
        byte alphaDepth; // 0 = no alpha, 1 = 1 Bit, 4 = Bit (only DXT3), 8 = 8 Bit Alpha
        byte alphaEncoding; // 0: DXT1 alpha (0 or 1 Bit alpha), 1 = DXT2/3 alpha (4 Bit), 7: DXT4/5 (interpolated alpha)
        byte hasMipmaps; // If true (1), then there are Mipmaps
        int width; // X Resolution of the biggest Mipmap
        int height; // Y Resolution of the biggest Mipmap

        uint[] mipmapOffsets = new uint[16]; // Offset for every Mipmap level. If 0 = no more mitmap level
        uint[] mippmapSize = new uint[16]; // Size for every level
        ARGBColor8[] paletteBGRA = new ARGBColor8[256]; // The color-palette for non-compressed pictures

        Stream str;

        private byte[] getPictureUncompressedByteArray(int MipmapLevel)
        {
            if (MipmapLevel >= MipMapCount)
            {
                MipmapLevel = MipMapCount - 1;
            }

            if (MipmapLevel < 0)
            {
                MipmapLevel = 0;
            }

            byte[] pic = new byte[width * height * 4 / (int)Math.Pow(2, MipmapLevel)];
            byte[] indices = getPictureData(MipmapLevel);
            for (int i = 0; i < indices.Length; i++)
            {
                pic[i * 4] = paletteBGRA[indices[i]].red;
                pic[(i * 4) + 1] = paletteBGRA[indices[i]].green;
                pic[(i * 4) + 2] = paletteBGRA[indices[i]].blue;
                pic[(i * 4) + 3] = (alphaDepth > 0) ? paletteBGRA[indices[i]].alpha : (byte)255;
            }
            return pic;
        }

        private byte[] getPictureData(int MipmapLevel)
        {
            if (str != null)
            {
                byte[] data;
                if (MipmapLevel >= MipMapCount)
                {
                    MipmapLevel = MipMapCount - 1;
                }

                if (MipmapLevel < 0)
                {
                    MipmapLevel = 0;
                }

                data = new byte[mippmapSize[MipmapLevel]];
                str.Position = (int)mipmapOffsets[MipmapLevel];
                str.ReadExactly(data);
                return data;
            }
            return null;
        }

        public int MipMapCount
        {
            get
            {
                int i = 0;
                while (mipmapOffsets[i] != 0)
                {
                    i++;
                }

                return i;
            }
        }

        public BlpFile(Stream _str)
        {
            str = _str;
            byte[] buffer = new byte[4];
            str.ReadExactly(buffer, 0, 4);

            if (new ASCIIEncoding().GetString(buffer) != "BLP2")
            {
                throw new Exception("Invalid BLP Format");
            }

            str.ReadExactly(buffer, 0, 4);
            type = BitConverter.ToUInt32(buffer, 0);
            if (type != 1)
            {
                throw new Exception("Invalid BLP-Type! Should be 1 but " + type + " was found");
            }

            str.ReadExactly(buffer, 0, 4);
            encoding = buffer[0];
            alphaDepth = buffer[1];
            alphaEncoding = buffer[2];
            hasMipmaps = buffer[3];

            str.ReadExactly(buffer, 0, 4);
            width = BitConverter.ToInt32(buffer, 0);

            str.ReadExactly(buffer, 0, 4);
            height = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < 16; i++)
            {
                _str.ReadExactly(buffer, 0, 4);
                mipmapOffsets[i] = BitConverter.ToUInt32(buffer, 0);
            }

            for (int i = 0; i < 16; i++)
            {
                str.ReadExactly(buffer, 0, 4);
                mippmapSize[i] = BitConverter.ToUInt32(buffer, 0);
            }

            if (encoding == 1)
            {
                for (int i = 0; i < 256; i++)
                {
                    byte[] color = new byte[4];
                    str.ReadExactly(color, 0, 4);
                    paletteBGRA[i].blue = color[0];
                    paletteBGRA[i].green = color[1];
                    paletteBGRA[i].red = color[2];
                    paletteBGRA[i].alpha = color[3];
                }
            }
        }

        public byte[] getImageBytes(int MipmapLevel)
        {
            byte[] pic;

            if (encoding == 2)
            {
                int flag = (alphaDepth > 1) ? ((alphaEncoding == 7) ? (int)DXTDecompression.DXTFlags.DXT5 : (int)DXTDecompression.DXTFlags.DXT3) : (int)DXTDecompression.DXTFlags.DXT1;
                DXTDecompression.DecompressImage(out pic, width / (int)Math.Pow(2, MipmapLevel), height / (int)Math.Pow(2, MipmapLevel), getPictureData(MipmapLevel), flag);
            }
            else
            {
                pic = getPictureUncompressedByteArray(MipmapLevel);
            }

            return pic;
        }

        public Bitmap GetBitmap(int MipmapLevel)
        {
            int x = width / (int)Math.Pow(2, MipmapLevel), y = height / (int)Math.Pow(2, MipmapLevel);
            Bitmap bmp = new(x, y);
            byte[] pic = getImageBytes(MipmapLevel);
            System.Drawing.Imaging.BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, x, y), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            ARGBColor8.convertToBGRA(ref pic);
            System.Runtime.InteropServices.Marshal.Copy(pic, 0, bmpdata.Scan0, pic.Length);
            bmp.UnlockBits(bmpdata);
            return bmp;
        }

        public void Dispose()
        {
            str?.Close();
        }
    }

    public static class DXTDecompression
    {
        public enum DXTFlags : int
        {
            DXT1 = 1 << 0,
            DXT3 = 1 << 1,
            DXT5 = 1 << 2,
        }

        private static void Decompress(ref byte[] rgba, byte[] block, int flags)
        {
            byte[] colourBlock = new byte[8];
            byte[] alphaBlock = block;

            if ((flags & ((int)DXTFlags.DXT3 | (int)DXTFlags.DXT5)) != 0)
            {
                Array.Copy(block, 8, colourBlock, 0, 8);
            }
            else
            {
                Array.Copy(block, 0, colourBlock, 0, 8);
            }

            DecompressColor(ref rgba, colourBlock, (flags & (int)DXTFlags.DXT1) != 0);

            if ((flags & (int)DXTFlags.DXT3) != 0)
            {
                DecompressAlphaDxt3(ref rgba, alphaBlock);
            }
            else if ((flags & (int)DXTFlags.DXT5) != 0)
            {
                DecompressAlphaDxt5(ref rgba, alphaBlock);
            }
        }

        private static void DecompressAlphaDxt3(ref byte[] rgba, byte[] block)
        {
            byte[] bytes = block;

            for (int i = 0; i < 8; i++)
            {
                byte quant = bytes[i];

                byte lo = (byte)(quant & 0x0F);
                byte hi = (byte)(quant & 0xF0);

                rgba[(8 * i) + 3] = (byte)(lo | (lo << 4));
                rgba[(8 * i) + 7] = (byte)(hi | (hi >> 4));
            }
        }

        private static void DecompressAlphaDxt5(ref byte[] rgba, byte[] block)
        {
            byte[] bytes = block;
            int alpha0 = bytes[0];
            int alpha1 = bytes[1];

            byte[] codes = new byte[8];
            codes[0] = (byte)alpha0;
            codes[1] = (byte)alpha1;

            if (alpha0 <= alpha1)
            {
                for (int i = 1; i < 5; i++)
                {
                    codes[1 + i] = (byte)((((5 - i) * alpha0) + (i * alpha1)) / 5);
                }

                codes[6] = 0;
                codes[7] = 255;
            }
            else
            {
                for (int i = 1; i < 7; i++)
                {
                    codes[i + 1] = (byte)((((7 - i) * alpha0) + (i * alpha1)) / 7);
                }
            }

            byte[] indices = new byte[16];
            byte[] blockSrc = bytes;
            int blockSrc_pos = 2;
            byte[] dest = indices;
            int indices_pos = 0;

            for (int i = 0; i < 2; i++)
            {
                int value = 0;

                for (int j = 0; j < 3; j++)
                {
                    int _byte = blockSrc[blockSrc_pos++];
                    value |= _byte << (8 * j);
                }

                for (int j = 0; j < 8; j++)
                {
                    int index = (value >> (3 * j)) & 0x07;
                    dest[indices_pos++] = (byte)index;
                }
            }

            for (int i = 0; i < 16; i++)
            {
                rgba[(4 * i) + 3] = codes[indices[i]];
            }
        }

        private static void DecompressColor(ref byte[] rgba, byte[] block, bool isDxt1)
        {
            byte[] bytes = block;

            byte[] codes = new byte[16];
            int a = Unpack565(bytes, 0, ref codes, 0);
            int b = Unpack565(bytes, 2, ref codes, 4);

            for (int i = 0; i < 3; i++)
            {
                int c = codes[i];
                int d = codes[4 + i];

                if (isDxt1 && a <= b)
                {
                    codes[8 + i] = (byte)((c + d) / 2);
                    codes[12 + i] = 0;
                }
                else
                {
                    codes[8 + i] = (byte)(((2 * c) + d) / 3);
                    codes[12 + i] = (byte)((c + (2 * d)) / 3);
                }
            }

            codes[8 + 3] = 255;
            codes[12 + 3] = (isDxt1 && a <= b) ? (byte)0 : (byte)255;

            byte[] indices = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                byte packed = bytes[4 + i];

                indices[0 + (i * 4)] = (byte)(packed & 0x3);
                indices[1 + (i * 4)] = (byte)((packed >> 2) & 0x3);
                indices[2 + (i * 4)] = (byte)((packed >> 4) & 0x3);
                indices[3 + (i * 4)] = (byte)((packed >> 6) & 0x3);
            }

            for (int i = 0; i < 16; i++)
            {
                byte offset = (byte)(4 * indices[i]);
                for (int j = 0; j < 4; j++)
                {
                    rgba[(4 * i) + j] = codes[offset + j];
                }
            }
        }

        private static int Unpack565(byte[] packed, int packed_offset, ref byte[] colour, int colour_offset)
        {
            int value = packed[0 + packed_offset] | (packed[1 + packed_offset] << 8);

            byte red = (byte)((value >> 11) & 0x1F);
            byte green = (byte)((value >> 5) & 0x3F);
            byte blue = (byte)(value & 0x1F);

            colour[0 + colour_offset] = (byte)((red << 3) | (red >> 2));
            colour[1 + colour_offset] = (byte)((green << 2) | (green >> 4));
            colour[2 + colour_offset] = (byte)((blue << 3) | (blue >> 2));
            colour[3 + colour_offset] = 255;

            return value;
        }

        public static void DecompressImage(out byte[] rgba, int width, int height, byte[] blocks, int flags)
        {
            rgba = new byte[width * height * 4];

            byte[] sourceBlock = blocks;
            int sourceBlock_pos = 0;
            int bytesPerBlock = ((flags & (int)DXTFlags.DXT1) != 0) ? 8 : 16;

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    byte[] targetRGBA = new byte[4 * 16];
                    int targetRGBA_pos = 0;
                    byte[] sourceBlockBuffer = new byte[bytesPerBlock];
                    if (sourceBlock.Length == sourceBlock_pos)
                    {
                        continue;
                    }

                    Array.Copy(sourceBlock, sourceBlock_pos, sourceBlockBuffer, 0, bytesPerBlock);
                    Decompress(ref targetRGBA, sourceBlockBuffer, flags);

                    byte[] sourcePixel = new byte[4];

                    for (int py = 0; py < 4; py++)
                    {
                        for (int px = 0; px < 4; px++)
                        {
                            int sx = x + px;
                            int sy = y + py;
                            if (sx < width && sy < height)
                            {
                                int targetPixel = 4 * ((width * sy) + sx);
                                Array.Copy(targetRGBA, targetRGBA_pos, sourcePixel, 0, 4);
                                targetRGBA_pos += 4;

                                for (int i = 0; i < 4; i++)
                                {
                                    rgba[targetPixel + i] = sourcePixel[i];
                                }
                            }
                            else
                            {
                                targetRGBA_pos += 4;
                            }
                        }
                    }

                    sourceBlock_pos += bytesPerBlock;
                }
            }
        }
    }
}
