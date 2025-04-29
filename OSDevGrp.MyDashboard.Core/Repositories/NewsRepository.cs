using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OSDevGrp.MyDashboard.Core.Repositories
{
    public class NewsRepository : INewsRepository
    {
        #region Private variables

        private readonly IDataProviderFactory _dataProviderFactory;
        private readonly IExceptionHandler _exceptionHandler;

        #endregion

        #region Constructor

        public NewsRepository(IDataProviderFactory dataProviderFactory, IExceptionHandler exceptionHandler)
        {
            if (dataProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(dataProviderFactory));
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            _dataProviderFactory = dataProviderFactory;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<INews>> GetNewsAsync()
        {
            try
            {
                INewsProvider[] newsProviders = (await _dataProviderFactory.BuildNewsProvidersAsync())?.ToArray();
                if (newsProviders == null || newsProviders.Length == 0)
                {
                    return new List<INews>(0);
                }

                Task<IEnumerable<INews>>[] getNewsFromNewsProviderTasks = newsProviders.Select(GetNewsFromNewsProviderAsync).ToArray();
                await Task.WhenAll(getNewsFromNewsProviderTasks);

                return getNewsFromNewsProviderTasks
                    .Where(task => task.IsFaulted == false)
                    .SelectMany(task => task.Result)
                    .OrderByDescending(news => news.Timestamp)
                    .ToList();
            }
            catch (AggregateException ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return new List<INews>(0);
        }

        private async Task<IEnumerable<INews>> GetNewsFromNewsProviderAsync(INewsProvider newsProvider)
        {
            if (newsProvider == null)
            {
                throw new ArgumentNullException(nameof(newsProvider));
            }

            try
            {
                try
                {
                    using HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;

                    using HttpClient client = new HttpClient(clientHandler);
                    using HttpResponseMessage responseMessage = await client.GetAsync(newsProvider.Uri);
                    if (responseMessage.IsSuccessStatusCode == false)
                    {
                        return new List<INews>(0);
                    }

                    await using Stream responseStream = await responseMessage.Content.ReadAsStreamAsync();

                    XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                    using XmlReader xmlReader = XmlReader.Create(responseStream, xmlReaderSettings);

                    XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlReader.NameTable);
                    if (xmlNamespaceManager.HasNamespace("ns") == false)
                    {
                        xmlNamespaceManager.AddNamespace("ns", xmlReader.NamespaceURI);
                    }
                    if (xmlNamespaceManager.HasNamespace("media") == false)
                    {
                        xmlNamespaceManager.AddNamespace("media", "http://search.yahoo.com/mrss/");
                    }

                    return GenerateNews(newsProvider, xmlReader, xmlNamespaceManager);
                }
                catch (HttpRequestException ex)
                {
                    StringBuilder messageBuilder = new StringBuilder($"Unable to communicate with {newsProvider.Name} ({newsProvider.Uri}): {ex.Message}");

                    Exception baseException = ex.GetBaseException();
                    messageBuilder.Append($" ({baseException.Message})");

                    throw new Exception(messageBuilder.ToString(), ex);
                }
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleAsync(ex);
            }
            return new List<INews>(0);
        }

        private IEnumerable<INews> GenerateNews(INewsProvider newsProvider, XmlReader xmlReader, XmlNamespaceManager xmlNamespaceManager)
        {
            if (newsProvider == null) throw new ArgumentNullException(nameof(newsProvider));
            if (xmlReader == null) throw new ArgumentNullException(nameof(xmlReader));
            if (xmlNamespaceManager == null) throw new ArgumentNullException(nameof(xmlNamespaceManager));

            XmlDocument xmlDocument = new XmlDocument(xmlNamespaceManager.NameTable);
            xmlDocument.Load(xmlReader);

            IList<INews> news = new List<INews>();

            XmlNodeList items = xmlDocument.SelectNodes("/ns:rss/ns:channel/ns:item", xmlNamespaceManager);
            if (items == null)
            {
                return news;
            }
            foreach (XmlNode item in items)
            {
                try
                {
                    news.Add(GenerateNews(newsProvider, item, xmlNamespaceManager));
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).GetAwaiter().GetResult();
                }
            }

            return news;
        }

        private static INews GenerateNews(INewsProvider newsProvider, XmlNode item, XmlNamespaceManager xmlNamespaceManager)
        {
            if (newsProvider == null) throw new ArgumentNullException(nameof(newsProvider));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (xmlNamespaceManager == null) throw new ArgumentNullException(nameof(xmlNamespaceManager));

            try
            {
                string title = ReadChildNodeValue(item, "ns:title", xmlNamespaceManager);
                string description = ReadChildNodeValue(item, "ns:description", xmlNamespaceManager);
                string pubDate = ReadChildNodeValue(item, "ns:pubDate", xmlNamespaceManager);
                string link = ReadChildNodeValue(item, "ns:link", xmlNamespaceManager);
                string guid = ReadChildNodeValue(item, "ns:guid", xmlNamespaceManager);

                return new News(
                    GenerateIdentifier(guid),
                    GenerateContent(title),
                    GenerateContent(description),
                    GenerateTimestamp(pubDate),
                    newsProvider)
                {
                    Link = GenerateUri(link),
                    Author = ExtractAuthor(item, xmlNamespaceManager),
                    MediaUrl = ExtractMediaUri(item, xmlNamespaceManager)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to generate and add news for the item '{item.OuterXml}': {ex.Message}", ex);
            }
        }

        private static string ReadChildNodeValue(XmlNode item, string childNodeName, XmlNamespaceManager xmlNamespaceManager)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrWhiteSpace(childNodeName)) throw new ArgumentNullException(nameof(childNodeName));
            if (xmlNamespaceManager == null) throw new ArgumentNullException(nameof(xmlNamespaceManager));

            XmlNode childNode = item.SelectSingleNode(childNodeName, xmlNamespaceManager);
            if (childNode == null)
            {
                return null;
            }

            if (childNode.HasChildNodes)
            {
                if (childNode.FirstChild is XmlCDataSection xmlCDataSection)
                {
                    return xmlCDataSection.Data;
                }

                if (childNode.FirstChild is XmlText xmlText)
                {
                    return xmlText.Value;
                }
            }

            return childNode.InnerText;
        }

        private static string GenerateIdentifier(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("D") : value;
        }

        private static string GenerateContent(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static DateTime GenerateTimestamp(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (DateTime.TryParse(value, out DateTime result))
            {
                return result;
            }

            return Rfc822DateTimeParser.Parse(value);
        }

        private static Uri GenerateUri(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out var result))
            {
                return result;
            }

            return null;
        }

        private static IAuthor ExtractAuthor(XmlNode item, XmlNamespaceManager xmlNamespaceManager)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (xmlNamespaceManager == null) throw new ArgumentNullException(nameof(xmlNamespaceManager));

            XmlNode authorNode = item.SelectSingleNode("ns:author", xmlNamespaceManager);
            if (authorNode == null)
            {
                return null;
            }

            string name = ReadChildNodeValue(authorNode, "ns:name", xmlNamespaceManager);
            if (string.IsNullOrWhiteSpace(name) == false)
            {
                return new Author(name);
            }

            if (string.IsNullOrWhiteSpace(authorNode.InnerText) == false)
            {
                return new Author(authorNode.InnerText);
            }

            return null;
        }

        private static Uri ExtractMediaUri(XmlNode item, XmlNamespaceManager xmlNamespaceManager)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (xmlNamespaceManager == null) throw new ArgumentNullException(nameof(xmlNamespaceManager));

            XmlNode contentNone = item.SelectSingleNode("media:content", xmlNamespaceManager);
            if (contentNone != null && string.IsNullOrEmpty(contentNone.Attributes?["url"]?.Value) == false)
            {
                return GenerateUri(contentNone.Attributes["url"].Value);
            }

            XmlNode thumbnailNone = item.SelectSingleNode("media:thumbnail", xmlNamespaceManager);
            if (thumbnailNone != null && string.IsNullOrEmpty(thumbnailNone.Attributes?["url"]?.Value) == false)
            {
                return GenerateUri(thumbnailNone.Attributes?["url"]?.Value);
            }

            return null;
        }

        #endregion
    }
}