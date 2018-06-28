using System;
using System.Collections.Generic;
using System.Linq;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Web.Models;

namespace OSDevGrp.MyDashboard.Web.Factories
{
    public class RedditLinkToInformationViewModelBuilder : ViewModelBuilderBase<InformationViewModel, IRedditLink>
    {
        #region Privat variables

        private readonly IRandomizer _randomizer;

        #endregion

        #region Constructor

        public RedditLinkToInformationViewModelBuilder(IRandomizer randomizer)
        {
            if (randomizer == null)
            {
                throw new ArgumentNullException(nameof(randomizer));
            }

            _randomizer = randomizer;
        }

        #endregion

        #region Methods

        protected override InformationViewModel Build(IRedditLink link)
        {
            string title = link.Title;
            string selftextAsText = link.SelftextAsText;
            string subredditDisplayNamePrefixed = GetDisplayNamePrefixedForSubreddit(link.Subreddit);
            string author = link.Author;
            Uri url = link.Url;
            Uri imageUrl = GetImageUrl(link);

            return new InformationViewModel
            {
                InformationIdentifier = link.Identifier,
                Timestamp = link.CreatedTime,
                Header = string.IsNullOrWhiteSpace(title) ? null : title,
                Details = string.IsNullOrWhiteSpace(selftextAsText) ? null : selftextAsText,
                ImageUrl = imageUrl == null ? null : imageUrl.AbsoluteUri,
                Provider = string.IsNullOrWhiteSpace(subredditDisplayNamePrefixed) ? null : subredditDisplayNamePrefixed,
                Author = GetAuthor(author, subredditDisplayNamePrefixed),
                ExternalUrl = url == null ? "#" : url.AbsoluteUri
            };
        }

        private string GetDisplayNamePrefixedForSubreddit(IRedditSubreddit subreddit)
        {
            if (subreddit == null)
            {
                return null;
            }
            return subreddit.DisplayNamePrefixed;
        }

        private string GetAuthor(string author, string subredditDisplayNamePrefixed)
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(subredditDisplayNamePrefixed))
            {
                return author;
            }
            return $"{author} @ {subredditDisplayNamePrefixed}";
        }

        private Uri GetImageUrl(IRedditLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            IEnumerable<Uri> imageUrls = GetImagesUrls(link);
            if (imageUrls.Any() == false)
            {
                return null;
            }

            int elementNo = _randomizer.Next(0, imageUrls.Count() - 1);
            return imageUrls.ElementAt(elementNo);
        }

        private IEnumerable<Uri> GetImagesUrls(IRedditLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            IEnumerable<IRedditImage> images = link.Images;
            if (images != null && images.Any(image => image.Url != null))
            {
                return images.Where(image => image.Url != null)
                    .Select(image => image.Url)
                    .ToList();
            }

            Uri thumbnailUrl = link.ThumbnailUrl;
            if (thumbnailUrl == null)
            {
                return new List<Uri>(0);
            }
            return new List<Uri> {thumbnailUrl};
        }

        #endregion
    }
}