using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using OSDevGrp.MyDashboard.Core.Contracts.Factories;
using OSDevGrp.MyDashboard.Core.Contracts.Infrastructure;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Contracts.Repositories;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Core.Utilities;

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

        public Task<IEnumerable<INews>> GetNewsAsync()
        {
            return Task.Run<IEnumerable<INews>>(async () => 
            {
                try
                {
                    IEnumerable<INewsProvider> newsProviders = await _dataProviderFactory.BuildNewsProvidersAsync();
                    if (newsProviders == null || newsProviders.Any() == false)
                    {
                        return new List<INews>(0);
                    }

                    Task<IEnumerable<INews>>[] getNewsFromNewsProviderTasks = newsProviders.Select(GetNewsFromNewsProviderAsync).ToArray();
                    Task.WaitAll(getNewsFromNewsProviderTasks);

                    return getNewsFromNewsProviderTasks
                        .Where(task => task.IsFaulted == false)
                        .SelectMany(task => task.Result)
                        .OrderByDescending(news => news.Timestamp)
                        .ToList();
                }
                catch (AggregateException ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<INews>(0);
            });
        }

        private Task<IEnumerable<INews>> GetNewsFromNewsProviderAsync(INewsProvider newsProvider)
        {
            if (newsProvider == null)
            {
                throw new ArgumentNullException(nameof(newsProvider));
            }

            return Task.Run<IEnumerable<INews>>(async () => 
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage responseMessage = await client.GetAsync(newsProvider.Uri))
                        {
                            if (responseMessage.IsSuccessStatusCode == false)
                            {
                                return new List<INews>(0);
                            }
                            using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                            {
                                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                                using (XmlReader xmlReader = XmlReader.Create(responseStream, xmlReaderSettings))
                                {
                                    return GenerateNews(newsProvider, xmlReader);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
                return new List<INews>(0);
            });
        }

        private IEnumerable<INews> GenerateNews(INewsProvider newsProvider, XmlReader xmlReader)
        {
            if (newsProvider == null) throw new ArgumentNullException(nameof(newsProvider));
            if (xmlReader == null) throw new ArgumentNullException(nameof(xmlReader));

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlReader);

            IList<INews> news = new List<INews>();

            XmlNodeList items = xmlDocument.SelectNodes("/rss/channel/item");
            foreach (XmlNode item in items)
            {
                try
                {
                    news.Add(GenerateNews(newsProvider, item));
                }
                catch (Exception ex)
                {
                    _exceptionHandler.HandleAsync(ex).Wait();
                }
            }

            return news;
        }

        private INews GenerateNews(INewsProvider newsProvider, XmlNode item)
        {
            if (newsProvider == null) throw new ArgumentNullException(nameof(newsProvider));
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                string title = ReadChildNodeValue(item, "title");
                string description = ReadChildNodeValue(item, "description");
                string pubDate = ReadChildNodeValue(item, "pubDate");
                string link = ReadChildNodeValue(item, "link");
                string guid = ReadChildNodeValue(item, "guid");
                
                return new News(
                    GenerateIdentifier(guid),
                    GenerateContent(title),
                    GenerateContent(description),
                    GenerateTimestamp(pubDate),
                    newsProvider)
                {
                    Link = GenerateUri(link),
                    Author = ExtractAuthor(item)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to generate and add news for the item '{item.OuterXml}': {ex.Message}", ex);
            }
        }

        private string ReadChildNodeValue(XmlNode item, string childNodeName)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrWhiteSpace(childNodeName)) throw new ArgumentNullException(nameof(childNodeName));

            XmlNode childNode = item.SelectSingleNode(childNodeName);
            if (childNode == null)
            {
                return null;
            }

            if (childNode.HasChildNodes)
            {
                XmlCDataSection xmlCDataSection = childNode.FirstChild as XmlCDataSection;
                if (xmlCDataSection != null)
                {
                    return xmlCDataSection.Data;
                }

                XmlText xmlText = childNode.FirstChild as XmlText;
                if (xmlText != null)
                {
                    return xmlText.Value;
                }
            }

            return childNode.InnerText;
        }

        private string GenerateIdentifier(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("D") : value;
        }

        private string GenerateContent(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private DateTime GenerateTimestamp(string value)
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

        private Uri GenerateUri(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            Uri result;
            if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out result))
            {
                return result;
            }
            
            return null;
        }

        private IAuthor ExtractAuthor(XmlNode item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            XmlNode authorNode = item.SelectSingleNode("author");
            if (authorNode == null)
            {
                return null;
            }

            string name = ReadChildNodeValue(authorNode, "name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return new Author(name);
        }

        #endregion
    }
}