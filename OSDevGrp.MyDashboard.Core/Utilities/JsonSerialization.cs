using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace OSDevGrp.MyDashboard.Core.Utilities
{
   public static class JsonSerialization
   {
       public static byte[] ToByteArray<T>(T obj, DataContractJsonSerializerSettings serializerSettings = null) where T : class
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
               return memoryStream.ToArray();
           }
       }

       public static T FromByteArray<T>(byte[] byteArray, DataContractJsonSerializerSettings serializerSettings = null) where T : class
       {
           if (byteArray == null)
           {
               throw new ArgumentNullException(nameof(byteArray));
           }

           using (MemoryStream memoryStream = new MemoryStream(byteArray))
           {
               return FromStream<T>(memoryStream, serializerSettings);
           }
       }

       internal static string ToBase64<T>(T obj, DataContractJsonSerializerSettings serializerSettings = null) where T : class
       {
           if (obj == null)
           {
               throw new ArgumentNullException(nameof(obj));
           }

           return Convert.ToBase64String(ToByteArray<T>(obj, serializerSettings));
       }

       internal  static T FromBase64<T>(string base64, DataContractJsonSerializerSettings serializerSettings = null) where T : class
       {
           if (string.IsNullOrWhiteSpace(base64))
           {
               throw new ArgumentNullException(nameof(base64));
           }

           return FromByteArray<T>(Convert.FromBase64String(base64), serializerSettings);
       }

       internal static T FromStream<T>(Stream stream, DataContractJsonSerializerSettings serializerSettings = null) where T : class
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