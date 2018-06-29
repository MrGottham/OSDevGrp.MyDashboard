using System;
using OSDevGrp.MyDashboard.Web.Contracts.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class ImageViewModel<TViewModel> : IViewModel where TViewModel : IViewModel
    {
        #region Private constants

        private const int MaxWidth = 400;
        private const int MaxHeight = 250;

        #endregion

        #region Constructor

        public ImageViewModel(TViewModel viewModel, byte[] image)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            ViewModel = viewModel;

            IImageFormat imageFormat;
            using (Image<Rgba32> sourceImage = Image.Load(image, out imageFormat))
            {
                OriginalMimeType = imageFormat.DefaultMimeType;
                OriginalImageAsBase64 = sourceImage.ToBase64String(imageFormat);

                int centerX = sourceImage.Width / 2;
                int centerY = sourceImage.Height / 2;

                ResizeOptions resizeOptions = new ResizeOptions
                {
                    Compand = false,
                    Mode = ResizeMode.Crop,
                    Size = new Size(MaxWidth, MaxHeight)
                };
                using (Image<Rgba32> targetImage = sourceImage.Clone(img => img.Resize(resizeOptions)))
                {
                    TransformedMimeType = imageFormat.DefaultMimeType;
                    TransformedImageAsBase64 = targetImage.ToBase64String(imageFormat);
                }
            }
        }

        #endregion

        #region Properties

        public TViewModel ViewModel { get; private set; }

        public string MimeType
        {
            get
            {
                return string.IsNullOrWhiteSpace(TransformedMimeType) ? OriginalMimeType : TransformedMimeType;
            }
        }

        public string ImageAsBase64
        {
            get
            {
                return string.IsNullOrWhiteSpace(TransformedImageAsBase64) ? OriginalImageAsBase64 : TransformedImageAsBase64;
            }
        }

        public string OriginalMimeType { get; private set; }

        public string OriginalImageAsBase64 { get; private set; }

        public string TransformedMimeType { get; private set; }

        public string TransformedImageAsBase64 { get; private set; }

        #endregion
    }
}