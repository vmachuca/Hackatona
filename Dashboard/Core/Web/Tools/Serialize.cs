#region "Usings"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;

#endregion

#region "Implementation"

namespace Core.Web.Tools
{
    #region "Public Class"

    public class Serialize
    {
        #region "Public Methods"

        /// <summary>
        /// Converting a list to stream
        /// </summary>
        /// <typeparam name="T">Type list type</typeparam>
        /// <param name="list">The list</param>
        /// <returns>The stream</returns>
        public Stream ListToStream<T>(T list)
        {
            ///Creating the memory stream
            MemoryStream stream = new MemoryStream();

            ///Creating the serializer
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            ///Creating the Text eriter
            TextWriter textWriter = new StreamWriter(stream);

            ///Serializing
            serializer.Serialize(textWriter, list);

            ///Close the text writer
            textWriter.Close();

            ///Returning the bytes
            return new MemoryStream(stream.ToArray());
        }

        public Stream ObjectToJsonPStream<T>(T obj, string callback)
        {
            ///Creating the memory stream
            MemoryStream stream = new MemoryStream();

            ///Creating the Text eriter
            TextWriter textWriter = new StreamWriter(stream);

            //Creating the string builder
            StringBuilder stringBuilder = new StringBuilder();

            ///Creating the serializer
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            ///Setting the new max json length
            serializer.MaxJsonLength = Int32.MaxValue;

            ///Serializing
            serializer.Serialize(obj, stringBuilder);

            ///Close the text writer
            textWriter.Write(string.Concat(
                callback,
                "(",
                stringBuilder.ToString(),
                ")"));
            textWriter.Close();

            ///Returning the bytes
            return new MemoryStream(stream.ToArray());
        }

        public Stream ObjectToJsonPStream2<T>(T obj, string callback)
        {
            ///Creating the memory stream
            MemoryStream stream = new MemoryStream();

            ///Creating the Text eriter
            TextWriter textWriter = new StreamWriter(stream);

            //Serialize obj
            string objSerialized = JsonConvert.SerializeObject(obj);

            ///Close the text writer
            textWriter.Write(string.Concat(
                callback,
                "(",
                objSerialized,
                ")"));
            textWriter.Close();

            ///Returning the bytes
            return new MemoryStream(stream.ToArray());
        }

        public static T JsonToObject<T>(string json)
        {
                ///Creating a instance of the object
            T obj = Activator.CreateInstance<T>();

            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

        #endregion
    }

    #endregion
}

#endregion