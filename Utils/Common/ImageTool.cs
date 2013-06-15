using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Iridescent.Utils.Common
{
    public enum ZoomMode
    {
        Fixed,
        GeometricProportion
    }

    public class ImageTool : IDisposable
    {
        private Stream _stream;

        public Stream BaseStream
        {
            get { return _stream; }
        }

        public string BasePath { get; set; }

        public ImageTool(Stream stream, string basePath)
        {
            _stream = stream;
            BasePath = basePath;
        }

        public bool IsImageStream
        {
            get
            {
                if (_stream != null && _stream.Length > 0)
                {
                    int buffer;
                    buffer = _stream.ReadByte();
                    string fileClass = buffer.ToString();
                    buffer = _stream.ReadByte();
                    fileClass += buffer.ToString();

                    _stream.Position = 0;

                    //jpg || gif ||bmp ||png
                    if (fileClass == "255216" || fileClass == "7173" || fileClass == "6677" || fileClass == "13780")
                        return true;
                }

                return false;
            }
        }

        public static bool IsImageExtentions(string fileName)
        {
            string extensions = Path.GetExtension(fileName).ToLower();
            return (new string[] {".jpg", ".jpeg", ".bmp", ".png", ".gif"}).Contains(extensions);
        }

        /// <summary>
        /// 使用guid进行md5 16位加密获取唯一文件名，扩展名不变
        /// </summary>
        /// <param name="rawFileName"></param>
        /// <returns></returns>
        public static string GetUniqueFileName(string rawFileName)
        {
            return GetUniqueFileName() + Path.GetExtension(rawFileName).ToLower();
        }

        /// <summary>
        /// 使用guid进行md5 16位加密获取唯一文件名，无扩展名
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueFileName()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(bytes);
            return BitConverter.ToString(hash, 4, 8).Replace("-", "");
        }
        
        public static string GetThumbnailFileName(string rawFileName)
        {
            return Path.GetFileNameWithoutExtension(rawFileName) + ".thumb" + Path.GetExtension(rawFileName);
        }

        public static string GetRawFileName(string thumbnailFileName)
        {
            return Regex.Replace(thumbnailFileName, @"\.thumb(\.[^.]+)$", "$1");
        }

        private string GetFilePath(string fileName)
        {
            return BasePath + fileName;
        }

        public void Save(string fileName)
        {
            using (FileStream fileStream = File.OpenWrite(GetFilePath(fileName)))
            {
                int bytesLength = (int)_stream.Length;
                byte[] rawData = new byte[bytesLength];
                _stream.Read(rawData, 0, bytesLength);
                fileStream.Write(rawData, 0, bytesLength);
                _stream.Position = 0;
            }
        }

        public void Save(string fileName, int width, int height, ZoomMode zoomMode=ZoomMode.GeometricProportion)
        {
            using (Image originalImg = Image.FromStream(_stream))
            {
                Size size = originalImg.Size;
                switch (zoomMode)
                {
                    case ZoomMode.GeometricProportion:
                        if (size.Width > width || size.Height > height)
                        {
                            float scale = Math.Min((float)width / size.Width, (float)height / size.Height);
                            size = new Size((int)(scale * size.Width), (int)(scale * size.Height));
                        }
                        break;
                    case ZoomMode.Fixed:
                        size = new Size(width, height);
                        break;
                }

                using (Bitmap bmp = new Bitmap(size.Width, size.Height))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.Clear(Color.Transparent);
                    g.DrawImage(originalImg, 0, 0, size.Width, size.Height);

                    bmp.Save(GetFilePath(fileName),ImageFormat.Jpeg);
                }
            }
        }

        public virtual void Close()
        {
            Dispose(true);
        }

        public virtual void Dispose()
        {
            Close();
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                if (_stream != null)
                    _stream.Close();
            }
            _stream = null;
        }
    }
}
