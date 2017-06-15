/*
 * Copyright (C)  2011  Axel Kesseler
 * 
 * This software is free and you can use it for any purpose. Furthermore, 
 * you are free to copy, to modify and/or to redistribute this software.
 * 
 * In addition, this software is distributed in the hope that it will be 
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 */

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace plexdata.generic
{
    /// <summary>
    /// Generic class to serialize given objects.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Saves given root object including all sub-objects into an XML file.
        /// </summary>
        /// <typeparam name="T">Object type to be used.</typeparam>
        /// <param name="filename">Name of the output file to be used.</param>
        /// <param name="root">Object instance to be serialized.</param>
        /// <returns>True if successful and false otherwise.</returns>
        public static bool Save<T>(string filename, T root)
        {
            bool success = true; // Assume success!

            XmlSerializer serializer = null;
            TextWriter writer = null;

            try
            {
                serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filename);
                serializer.Serialize(writer, root);
            }
            catch (Exception exception)
            {
                Serializer.Exception = exception;
                success = false;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }

            writer = null;
            serializer = null;

            return success;
        }

        /// <summary>
        /// Loads given root object including all sub-objects from an XML file.
        /// </summary>
        /// <typeparam name="T">Object type to be used.</typeparam>
        /// <param name="filename">Name of the input file to be used.</param>
        /// <param name="root">Object instance to be deserialized.</param>
        /// <returns>True if successful and false otherwise.</returns>
        public static bool Load<T>(string filename, out T root)
        {
            bool success = true; // Assume success!

            XmlSerializer serializer = null;
            TextReader reader = null;

            root = default(T);

            try
            {
                serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filename);
                root = (T)serializer.Deserialize(reader);
            }
            catch (Exception exception)
            {
                Serializer.Exception = exception;
                success = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            reader = null;
            serializer = null;

            return success;
        }

        /// <summary>
        /// Instance of last occurred exception.
        /// </summary>
        private static Exception exception = null;
 
        /// <summary>
        /// Gets last exception if either serialization or de-serialization failed.
        /// </summary>
        public static Exception Exception
        {
            get { return Serializer.exception; }
            private set { Serializer.exception = value; if (value != null) { Debug.WriteLine(value.ToString()); } }
        }
    }
}
