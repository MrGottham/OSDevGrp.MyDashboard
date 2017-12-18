using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace OSDevGrp.MyDashboard.Core.Utilities
{
   public static class JsonSerialization
   {
       public static string ToBase64<T>(T obj, DataContractJsonSerializerSettings serializerSettings = null) where T : class
       {
           if (obj == null)
           {
               throw new ArgumentNullException(nameof(obj));
           }

           using (MemoryStream memoryStream = new MemoryStream())
           {
               DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType(), serializerSettings);
               serializer.WriteObject(memoryStream, obj);

               memoryStream.Seek(0, SeekOrigin.Begin);
               return Convert.ToBase64String(memoryStream.ToArray());
           }
       }

       public static T FromBase64<T>(string base64, DataContractJsonSerializerSettings serializerSettings = null) where T : class
       {
           if (string.IsNullOrWhiteSpace(base64))
           {
               throw new ArgumentNullException(nameof(base64));
           }

           using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64)))
           {
               return FromStream<T>(memoryStream, serializerSettings);
           }
       }
       
       public static T FromStream<T>(Stream stream, DataContractJsonSerializerSettings serializerSettings = null) where T : class
       {
           if (stream == null)
           {
               throw new ArgumentNullException(nameof(stream));
           }

           DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), serializerSettings);
           return (T) serializer.ReadObject(stream);
       }
   }
}
