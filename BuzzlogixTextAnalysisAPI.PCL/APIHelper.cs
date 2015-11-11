/*
 * BuzzlogixTextAnalysisAPI.PCL
 *
 * This file was automatically generated for Buzzlogix by APIMATIC BETA v2.0 on 11/09/2015
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using unirest_net.request;

namespace Buzzlogix
{
    static class APIHelper
    {
        /// <summary>
        /// JSON Serialization of a given object.
        /// </summary>
        /// <param name="obj">The object to serialize into JSON</param>
        /// <returns>The serialized Json string representation of the given object</returns>
        internal static string JsonSerialize(object obj)
        {
            if(null == obj)
                return null;
                
            return JsonConvert.SerializeObject
                (obj, Formatting.None, new JsonSerializerSettings() {
                    Converters = new List<JsonConverter> {
                        new Newtonsoft.Json.Converters.StringEnumConverter()
                }});
        }

        /// <summary>
        /// JSON Deserialization of the given json string.
        /// </summary>
        /// <param name="json">The json string to deserialize</param>
        /// <typeparam name="T">The type of the object to desialize into</typeparam>
        /// <returns>The deserialized object</returns>
        internal static T JsonDeserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Replaces template parameters in the given url
        /// </summary>
        /// <param name="queryUrl">The query url string to replace the template parameters</param>
        /// <param name="parameters">The parameters to replace in the url</param>        
        internal static void AppendUrlWithTemplateParameters
            (StringBuilder queryBuilder, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            //perform parameter validation
            if (null == queryBuilder)
                throw new ArgumentNullException("queryBuilder");

            if (null == parameters)
                return;

            //iterate and replace parameters
            foreach(KeyValuePair<string, object> pair in parameters)
            {                
                string replaceValue = string.Empty;

                //load element value as string
                if (null == pair.Value)
                    replaceValue = "";
                else if (pair.Value is ICollection)
                    replaceValue = flattenCollection(pair.Value as ICollection, "{0}{1}", '/');
                else
                    replaceValue = pair.Value.ToString();

                //find the template parameter and replace it with its value
                queryBuilder.Replace(string.Format("{{{0}}}", pair.Key), replaceValue);
            }
        }

        /// <summary>
        /// Appends the given set of parameters to the given query string
        /// </summary>
        /// <param name="queryUrl">The query url string to append the parameters</param>
        /// <param name="parameters">The parameters to append</param>        
        internal static void AppendUrlWithQueryParameters
            (StringBuilder queryBuilder, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            //perform parameter validation
            if (null == queryBuilder)
                throw new ArgumentNullException("queryBuilder");
                
            if (null == parameters)
                return;

            //does the query string already has parameters
            bool hasParams = (indexOf(queryBuilder, "?") > 0);
            
            //iterate and append parameters
            foreach (KeyValuePair<string, object> pair in parameters)
            {
                //ignore null values
                if (pair.Value == null)
                    continue;

                //if already has parameters, use the &amp; to append new parameters
                queryBuilder.Append((hasParams) ? '&' : '?');
                
                //indicate that now the query has some params
                hasParams = true;

                string paramKeyValPair;

                //load element value as string
                if (pair.Value is ICollection)
                    paramKeyValPair = flattenCollection(pair.Value as ICollection, string.Format("{0}[]={{0}}{{1}}", pair.Key), '&');
                else
                    paramKeyValPair = string.Format("{0}={1}", pair.Key, pair.Value.ToString());
                
                //append keyval pair for current parameter
                queryBuilder.Append(paramKeyValPair);
            }
        }

        /// <summary>
        /// StringBuilder extension method to implement IndexOf functionality.
        /// This does a StringComparison.Ordinal kind of comparison.
        /// </summary>
        /// <param name="stringBuilder">The string builder to find the index in</param>
        /// <param name="strCheck">The string to locate in the string builder</param>
        /// <returns>The index of string inside the string builder</returns>
        private static int indexOf(StringBuilder stringBuilder, string strCheck)
        {
            if (stringBuilder == null)
                throw new ArgumentNullException("stringBuilder");

            if (strCheck == null)
                return 0;

            //iterate over the input
            for (int inputCounter = 0; inputCounter < stringBuilder.Length; inputCounter++)
            {
                int matchCounter;

                //attempt to locate a potential match
                for (matchCounter = 0;
                        (matchCounter < strCheck.Length)
                        && (inputCounter + matchCounter < stringBuilder.Length)
                        && (stringBuilder[inputCounter + matchCounter] == strCheck[matchCounter]);
                    matchCounter++); 
                
                //verify the match
                if (matchCounter == strCheck.Length)
                    return inputCounter;
            }

            return -1;
        }

        /// <summary>
        /// Validates and processes the given query Url to clean empty slashes
        /// </summary>
        /// <param name="queryBuilder">The given query Url to process</param>
        /// <returns>Clean Url as string</returns>
        internal static string CleanUrl(StringBuilder queryBuilder)
        {
            //convert to immutable string
            string url = queryBuilder.ToString();

            //ensure that the urls are absolute            
            Match protocol = Regex.Match(url, "^https?://[^/]+");
            if (!protocol.Success)
                throw new ArgumentException("Invalid Url format.");

            //remove redundant forward slashes
            string query = url.Substring(protocol.Length);
            query = Regex.Replace(query, "//+", "/");

            //return process url
            return string.Concat(protocol.Value, query);
        }

        /// <summary>
        /// A neat way of parsing string to enum values
        /// </summary>
        /// <param name="sEnumValue">String value to parse</param>
        /// <returns>Parsed enum value in the given type</returns>
        internal static TEnum ParseEnum<TEnum>(string sEnumValue) where TEnum : struct
        {
            TEnum eTemp;
            if (Enum.TryParse<TEnum>(sEnumValue, true, out eTemp) == true)
                return eTemp;

            throw new ArgumentOutOfRangeException(
                string.Format("Value \"{0}\" is not defined in {1}", sEnumValue, typeof(TEnum)));
        }

        /// <summary>
        /// Used for flattening a collection of objects into a string 
        /// </summary>
        /// <param name="array">Array of elements to flatten</param>
        /// <param name="fmt">Format string to use for array flattening</param>
        /// <param name="separator">Separator to use for string concat</param>
        /// <returns>Representative string made up of array elements</returns>
        private static string flattenCollection(ICollection array, string fmt, char separator)
        {
            StringBuilder builder = new StringBuilder();

            //append all elements in the array into a string
            foreach (object element in array)
            {
                string elemValue = null;

                //replace null values with empty string to maintain index order
                if (null == element)
                    elemValue = string.Empty;
                else
                    elemValue = element.ToString();
                    
                builder.AppendFormat(fmt, elemValue, separator);
            }

            //remove the last separator, if appended
            if ((builder.Length > 1) && (builder[builder.Length - 1] == separator))
                builder.Length -= 1;

            return builder.ToString();
        }

        /// <summary>
        /// Prepares Array style form fields from a given array of values
        /// </summary>
        /// <param name="name">Name of the form field</param>
        /// <param name="values">Values for the array form field</param>
        /// <returns>Dictionary of form fields created from array elements</returns>
        internal static Dictionary<string, object> PrepareFormFieldsFromArray(string name, ICollection values)
        {
            Dictionary<string, object> formFields = new Dictionary<string, object>();

            //counter for array index
            int index = 0;

            //iterate over all elements and create form array fields
            foreach (object element in values)
            {
                string elemValue = null;

                //replace null values with empty string to maintain index order
                if (null == element)
                    elemValue = string.Empty;
                else
                    elemValue = element.ToString();
                    
                formFields.Add(string.Format("{0}[{1}]", name, index++), elemValue);
            }

            return formFields;
        }
    }
}
