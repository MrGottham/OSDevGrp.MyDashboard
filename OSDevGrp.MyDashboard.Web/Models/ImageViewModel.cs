using OSDevGrp.MyDashboard.Web.Contracts.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class ImageViewModel<TViewModel> : IViewModel where TViewModel : IViewModel
    {
        #region Private constants

        private const int MaxWidth = 448;
        private const int MaxHeight = 268;

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

            using (Image sourceImage = Image.Load(image))
            {
                IImageFormat imageFormat = sourceImage.Metadata.DecodedImageFormat!;

                OriginalMimeType = imageFormat.DefaultMimeType;
                OriginalImageAsBase64 = sourceImage.ToBase64String(imageFormat);

                ResizeOptions resizeOptions = new ResizeOptions
                {
                    Compand = false,
                    Mode = ResizeMode.Crop,
                    Size = new Size(MaxWidth, MaxHeight)
                };
                using (Image targetImage = sourceImage.Clone(img => img.Resize(resizeOptions)))
                {
                    TransformedMimeType = imageFormat.DefaultMimeType;
                    TransformedImageAsBase64 = targetImage.ToBase64String(imageFormat);
                }
            }
        }

        #endregion

        #region Properties

        public TViewModel ViewModel { get; private set; }

        public string MimeType => string.IsNullOrWhiteSpace(TransformedMimeType) ? OriginalMimeType : TransformedMimeType;

        public string ImageAsBase64 => string.IsNullOrWhiteSpace(TransformedImageAsBase64) ? OriginalImageAsBase64 : TransformedImageAsBase64;

        public string OriginalMimeType { get; }

        public string OriginalImageAsBase64 { get; }

        public string TransformedMimeType { get; }

        public string TransformedImageAsBase64 { get; }

        #endregion
    }
}