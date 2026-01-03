using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FractalVision.Views;

namespace FractalVision.Services
{
    public class ImageExporter
    {
        private readonly FractalViewer _fractalViewer;

        public ImageExporter(FractalViewer fractalViewer)
        {
            _fractalViewer = fractalViewer;
        }

        public string ExportToPng(string filePath, int? width = null, int? height = null)
        {
            return ExportImage(filePath, ImageFormat.Png, width, height);
        }

        public string ExportToJpeg(string filePath, int quality = 90, int? width = null, int? height = null)
        {
            return ExportImage(filePath, ImageFormat.Jpeg, width, height, quality);
        }


        private string ExportImage(string filePath, ImageFormat format, int? width, int? height, int jpegQuality = 90)
        {
            try
            {
                using (var original = _fractalViewer.FractalImage)
                {
                    Bitmap imageToSave;

                    if (width.HasValue && height.HasValue &&
                        (width.Value != original.Width || height.Value != original.Height))
                    {
                        imageToSave = ResizeImage(original, width.Value, height.Value);
                    }
                    else
                    {
                        imageToSave = new Bitmap(original);
                    }

                    if (format == ImageFormat.Jpeg)
                    {
                        var encoder = ImageCodecInfo.GetImageEncoders()
                            .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

                        if (encoder != null)
                        {
                            var encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(
                                Encoder.Quality,
                                Math.Min(100, Math.Max(10, jpegQuality))
                            );

                            imageToSave.Save(filePath, encoder, encoderParams);
                        }
                        else
                        {
                            imageToSave.Save(filePath, format);
                        }
                    }
                    else
                    {
                        imageToSave.Save(filePath, format);
                    }

                    imageToSave.Dispose();
                }

                return $"Изображение сохранено: {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                return $"Ошибка сохранения: {ex.Message}";
            }
        }

        private Bitmap ResizeImage(Image original, int width, int height)
        {
            var resized = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                graphics.DrawImage(original, 0, 0, width, height);
            }

            return resized;
        }

        public string GetDefaultFileName()
        {
            var type = _fractalViewer.FractalType.ToString().ToLower();
            var zoom = _fractalViewer.Parameters.Zoom.ToString("F2").Replace(".", "_");
            var date = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return $"fractal_{type}_zoom{zoom}_{date}";
        }

        public Size GetCurrentSize()
        {
            var image = _fractalViewer.FractalImage;
            return new Size(image.Width, image.Height);
        }

        public void CopyToClipboard()
        {
            try
            {
                using (var image = _fractalViewer.FractalImage)
                {
                    Clipboard.SetImage(image);
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки буфера обмена
            }
        }
    }
}