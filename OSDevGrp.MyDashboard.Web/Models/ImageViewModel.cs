using System;
using OSDevGrp.MyDashboard.Web.Contracts.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace OSDevGrp.MyDashboard.Web.Models
{
    public class ImageViewModel<TViewModel> : IViewModel where TViewModel : IViewModel
    {
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
            using (Image<Rgba32> img = Image.Load(image, out imageFormat))
            {
                OriginalMimeType = imageFormat.DefaultMimeType;
                OriginalImageAsBase64 = img.ToBase64String(imageFormat);
            }
        }

        #endregion

        #region Properties

        public TViewModel ViewModel { get; private set; }

        public string OriginalMimeType { get; private set; }

        public string OriginalImageAsBase64 { get; private set; }

        #endregion
    }
}