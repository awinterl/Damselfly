﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Damselfly.Core.Interfaces;
using Damselfly.Core.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Damselfly.Core.ImageProcessing
{
    public class ImageSharpProcessor : IImageProcessor
    {
        private static FontCollection fontCollection;
        private static readonly string[] s_imageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public ICollection<string> SupportedFileExtensions { get { return s_imageExtensions;  } }

        public ImageSharpProcessor()
        {

            // lets switch out the default encoder for jpeg to one
            // that saves at 90 quality
            Configuration.Default.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder()
            {
                Quality = 90
            });

        }
        /// <summary>
        /// Initialises and installs the font for the watermarking.
        /// TODO: In future we'll make the font configurable.
        /// </summary>
        /// <param name="folder"></param>
        public void SetFontPath(string folder)
        {
            try
            {
                fontCollection = new FontCollection();

                string fontPath = Path.Combine(folder, "arial.ttf");

                fontCollection.Install(fontPath);

                Logging.Log("Watermark font installed: {0}", fontPath);
            }
            catch (Exception ex)
            {
                Logging.LogError($"Exception installing watermark font: {ex.Message}");
            }
        }

        /// <summary>
        /// Create an SHA1 hash from the image data (pixels only) to allow us to find
        /// duplicate images. Note that this ignores EXIF metadata, so the hash will
        /// find duplicate images even if the metadata is different.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>String hash of the image data</returns>
        public static string GetHash(Image<Rgba32> image)
        {
            string result = null;

            try
            {
                var hashWatch = new Stopwatch("CalcImageHash");

                var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);

                for (int y = 0; y < image.Height; y++)
                {
                    Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y);

                    byte[] rgbaBytes = MemoryMarshal.AsBytes(pixelRowSpan).ToArray();
                    hash.AppendData(rgbaBytes);
                }

                result = hash.GetHashAndReset().ToHex(true);
                hashWatch.Stop();
                Logging.LogVerbose($"Hashed image ({result}) in {hashWatch.HumanElapsedTime}");
            }
            catch ( Exception ex )
            {
                Logging.LogError($"Exception while calculating hash: {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// Resize using SixLabors ImageSharp, which can do 100 images in about 59s (2020 MacBook Air i5)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destFiles"></param>
        public Task<ImageProcessResult> CreateThumbs(FileInfo source, IDictionary<FileInfo, ThumbConfig> destFiles)
        {
            var result = new ImageProcessResult();
            Stopwatch load = new Stopwatch("ImageSharpLoad");

            // Image.Load(string path) is a shortcut for our default type. 
            // Other pixel formats use Image.Load<TPixel>(string path))
            using var image = Image.Load<Rgba32>(source.FullName);

            // We've got the image in memory. Create the hash. 
            result.ImageHash = GetHash(image);

            load.Stop();

            Stopwatch orient = new Stopwatch("ImageSharpOrient");

            image.Mutate(x => x.AutoOrient());

            orient.Stop();

            Stopwatch thumbs = new Stopwatch("ImageSharpThumbs");

            foreach (var pair in destFiles)
            {
                var dest = pair.Key;
                var config = pair.Value;
                var mode = ResizeMode.Max;

                var size = new Size { Height = config.height, Width = config.width };

                Logging.LogTrace("Generating thumbnail for {0}: {1}x{2}", source.Name, size.Width, size.Height);

                if (config.cropToRatio)
                {
                    // For the smallest thumbs, we crop to fix the aspect exactly.
                    mode = ResizeMode.Crop;
                }

                var opts = new ResizeOptions { Mode = mode, Size = size };

                // Note, we don't clone and resize from the original image, because that's expensive.
                // So we always resize the previous image, which will be faster for each iteration
                // because each previous image is progressively smaller. 
                image.Mutate(x => x.Resize(opts));
                image.Save(dest.FullName);

                result.ThumbsGenerated = true;
            }

            thumbs.Stop();

            return Task.FromResult(result);
        }

        /// <summary>
        /// Transforms an image to add a watermark.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="waterMarkText"></param>
        public void TransformDownloadImage(string input, Stream output, string waterMarkText = null)
        {
            Logging.Log($" Running image transform for Watermark: {waterMarkText}");

            using var img = Image.Load(input, out IImageFormat fmt);

            var opts = new ResizeOptions { Mode = ResizeMode.Max, Size = new Size { Height = 1600, Width = 1600 } };

            // First rotate and resize.
            img.Mutate(x => x.AutoOrient().Resize(opts));

            if (!string.IsNullOrEmpty(waterMarkText))
            {
                // Apply the watermark if one's been specified.
                Font font = fontCollection.CreateFont("Arial", 10);

                if (!string.IsNullOrEmpty(waterMarkText))
                    img.Mutate(context => ApplyWaterMark(context, font, waterMarkText, Color.White));
            }

            img.Save(output, fmt);
        }


        /// <summary>
        /// Given a SixLabours ImageSharp image context, applies a watermark text overlay
        /// to the bottom right corner in the given font and colour. 
        /// </summary>
        /// <param name="processingContext"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private static IImageProcessingContext ApplyWaterMark(IImageProcessingContext processingContext,
                                                        Font font, string text, Color color)
        {
            Size imgSize = processingContext.GetCurrentSize();

            // measure the text size
            FontRectangle size = TextMeasurer.Measure(text, new RendererOptions(font));

            int ratio = 4; // Landscape, we make the text 25% of the width

            if (imgSize.Width >= imgSize.Height)
            {
                // Landscape - make it 1/6 of the width
                ratio = 6;
            }

            float quarter = imgSize.Width / ratio;

            // We want the text width to be 25% of the width of the image
            float scalingFactor = quarter / size.Width;

            // create a new font
            Font scaledFont = new Font(font, scalingFactor * font.Size);

            // 5% padding from the edge
            float fivePercent = quarter / 20;

            // 5% from the bottom right.
            var position = new PointF(imgSize.Width - fivePercent, imgSize.Height - fivePercent);

            var textGraphicOptions = new DrawingOptions
            {
                TextOptions = {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                }
            };
            return processingContext.DrawText(textGraphicOptions, text, scaledFont, color, position);
        }
    }
}
