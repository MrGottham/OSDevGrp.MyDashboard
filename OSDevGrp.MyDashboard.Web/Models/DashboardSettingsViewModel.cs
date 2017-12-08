using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using OSDevGrp.MyDashboard.Core.Contracts.Models;
using OSDevGrp.MyDashboard.Core.Models;
using OSDevGrp.MyDashboard.Web.Contracts.Models;

namespace OSDevGrp.MyDashboard.Web.Models
{
    [DataContract(Name = "DashboardSettings", Namespace = "urn:osdevgrp:mydashboard:1.0.0")]
    public class DashboardSettingsViewModel : IViewModel
    {
        #region Properties

        [Range(0, 250)]
        [Display(Name="Number of news", ShortName="News", Description="Number of news to receive")]
        [DataMember(Name = "NumberOfNews")]
        public int NumberOfNews { get; set; }
        
        [Display(Name="Use Reddit", ShortName="Reddit", Description="Collection data from Reddit")]
        [DataMember(Name = "UseReddit")]
        public bool UseReddit { get; set; }

        #endregion

        #region Methods

        public IDashboardSettings ToDashboardSettings()
        {
            return new DashboardSettings()
            {
                NumberOfNews = NumberOfNews,
                UseReddit = UseReddit
            };
        }

        public string ToBase64()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DashboardSettingsViewModel));
                serializer.WriteObject(memoryStream, this);

                memoryStream.Seek(0, SeekOrigin.Begin);
                return Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.None);
            }
        }

        public static DashboardSettingsViewModel Create(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                throw new ArgumentNullException(nameof(base64));
            }

            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DashboardSettingsViewModel));
                return serializer.ReadObject(memoryStream) as DashboardSettingsViewModel;
            }
        }

        #endregion
    }
}
