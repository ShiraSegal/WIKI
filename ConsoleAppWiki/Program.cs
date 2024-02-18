using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json.Linq;

namespace ConsoleAppWiki
{
    class Program
    {
        static string connectionStringPlace_new = @"Password=Yv2017N9!;Max Pool Size=1024;Connect Timeout=60;Persist Security Info=True;User ID=yvng2017;Initial Catalog=Place_new;Data Source=172.24.50.194";
        static string connectionStringTabularValues = @"Password=Yv2017N9!;Max Pool Size=1024;Connect Timeout=60;Persist Security Info=True;User ID=yvng2017;Initial Catalog=TabularValues;Data Source=172.24.50.194";



        public static void Main(string[] args)
        {
            string selectQuery = @"select
                                   case
                                       when DESCR LIKE '%,%' then left(DESCR, charindex(',', DESCR) -1)
                                   	else DESCR

                                       end
                                        as Title,
                                   CASE
                                       WHEN LANG = 'ENG' THEN 'en'

                                       WHEN LANG = 'HEB' THEN 'he'
                                   END as LANG, t.BOOK_ID
                                   from[PLACELISTTITLES] t
                                   LEFT OUTER JOIN PLACE_FD_SUMMARY_ENG s
                                   on t.BOOK_ID = s.book_id
                                   where s.book_id is null
                                   and lang in ('HEB')
                                   and DESCR is not null";

            var dataTable = GetDataTable(selectQuery);
            //List<WikiObj> wikiObjs = new List<WikiObj>();
            foreach (DataRow row in dataTable.Rows)
            {
                //wikiObjs.Add(new WikiObj
                //{
                //    Title = row["Title"].ToString(),
                //    Lang = row["Lang"].ToString(),
                //    BOOK_ID = row["BOOK_ID"].ToString()
                //});
                Test(row["Title"].ToString(), row["BOOK_ID"].ToString(), row["Lang"].ToString());
            }
            //Test("Rovnoye", "13211774", "en");

        }
        static async System.Threading.Tasks.Task Test(string title, string bookId, string lang)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            string summary = "";
            //string url = $"https://{lang}.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles={title}&cmtitle=Category:Places";
            string url = $"https://{lang}.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles={title}&rvslots=*&rvprop=content&formatversion=2&cmtitle=Category:Places";
            HttpResponseMessage response = client.GetAsync(url).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body.
                var dataObjects = await response.Content.ReadAsStringAsync();  //Make sure to add a reference to System.Net.Http.Formatting.dll

                if (dataObjects.Contains("query\":{\"pages\":{\"-1\":{\""))
                {
                    summary = "-1 URL:" + url;
                }
                else
                {
                    try
                    {
                        //dynamic data = JObject.Parse(dataObjects);
                        //Console.WriteLine("1: " + data.query);
                        //Console.WriteLine("2: " + data.query.pages);
                        ////Console.WriteLine("3: " + data.query.pages.First);
                        //dynamic data1 = JObject.Parse(data.query.pages.ToString());
                        //dynamic data2 = JObject.Parse(data1.First);
                        //Console.WriteLine("3.1: " + data.query.pages[0]);
                        //var obj = data.query.pages.select();
                        int startPos = dataObjects.LastIndexOf("\"extract\":") + "\"extract\":".Length + 1;
                        //int length = dataObjects.IndexOf("\"}}}") - startPos;
                        int length = dataObjects.Length - 1 - startPos;
                        summary = dataObjects.Substring(startPos, length);
                        summary = summary.Replace("\"}}", string.Empty).Replace("}",string.Empty);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("in try chtch: "+e);
                    }    
                  
                }
                summary = summary.Replace("'","''");
                title = title.Replace("'","''");
                string query = $"insert into [PLACE_WIKI_TEST_8_1_23] values (N'{summary}',N'{title}','{bookId}','{lang}','',GETDATE())";
                InsertRow(query);
                client.Dispose();
            }
            else
            {
                summary = response.StatusCode.ToString()+ "-|-" + response.RequestMessage;
                summary = summary.Replace("'", "''");
                title = title.Replace("'", "''");
                //var a = await response.Content.ReadAsStreamAsync();
                string moreInfo = "response.Content: " + response.Content + "-|-" +
                                  "response.Headers: " + response.Headers + " -|-" +
                                  "response.ReasonPhrase: " + response.ReasonPhrase;
                                  //"a: "+ a;
                
                string query = $"insert into [PLACE_WIKI_TEST_8_1_23] values (N'{summary}',N'{title}','{bookId}','{lang}','{moreInfo.Replace("'", "''")}',GETDATE())";
                InsertRow(query);
            }
        }

        static void exel(string bookid, string lang, string sammmery)
        {
            string Text = "bookId: " + bookid + ",|  lang: " + lang + ",|  Sammery: " + sammmery + Environment.NewLine;
            //File.WriteAllText("D:\\C# Projects\\IT Projects\\WIKITEST.test.text", createText);

            using (StreamWriter writetext = new StreamWriter("D:\\C# Projects\\IT Projects\\WIKITEST\\test.text", true, Encoding.UTF8))
            {
                // Console.SetOut(new StreamWriter(File.Create("d:/your_output.txt"), Encoding.UTF8) { AutoFlush = true });
                writetext.WriteLine(Text);
            }

            // //OleDbCommand cmd1 = new OleDbCommand("INSERT INTO [yourNamedRange] " +
            //   //         "VALUES(@value1, @value2, @value3, @value4)", cn);
            //string fileName = @"D:\test.xlsx";
            //string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;" +
            //        "Data Source={0};Extended Properties='Excel 12.0;HDR=YES;IMEX=0'", fileName);
            //using (OleDbConnection cn = new OleDbConnection(connectionString))
            //{
            //    cn.Open();
            //    OleDbCommand cmd1 = new OleDbCommand("INSERT INTO [Sheet1$] " +
            //         "([Column1],[Column2],[Column3],[Column4]) " +
            //         "VALUES(@value1, @value2, @value3, @value4)", cn);
            //    cmd1.Parameters.AddWithValue("@value1", "Key1");
            //    cmd1.Parameters.AddWithValue("@value2", "Sample1");
            //    cmd1.Parameters.AddWithValue("@value3", 1);
            //    cmd1.Parameters.AddWithValue("@value4", 9);
            //    cmd1.ExecuteNonQuery();
            //}
        }
        static string Convert(string unicodeString)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte array.
            byte[] unicodeBytes = unicode.GetBytes(unicodeString);

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // Convert the new byte[] into a char[] and then into a string.
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            return new string(asciiChars);

        }
        static string ConvertUTF8(string unicodeString)
        {
            Encoding ascii = Encoding.ASCII;
            string gkNumber = Char.ConvertFromUtf32(0x10154);
            char[] chars = new char[] { 'z', 'a', '\u0306', '\u01FD', '\u03B2',
                                  gkNumber[0], gkNumber[1] };

            // Get UTF-8 and UTF-16 encoders.
            Encoding utf8 = Encoding.UTF8;
            Encoding utf16 = Encoding.Unicode;

            // Display the original characters' code units.
            Console.WriteLine("Original UTF-16 code units:");
            byte[] utf16Bytes = utf16.GetBytes(chars);
            foreach (var utf16Byte in utf16Bytes)
                Console.Write("{0:X2} ", utf16Byte);
            Console.WriteLine();

            // Display the number of bytes required to encode the array.
            int reqBytes = utf8.GetByteCount(chars);
            Console.WriteLine("\nExact number of bytes required: {0}",
                          reqBytes);

            // Display the maximum byte count.
            int maxBytes = utf8.GetMaxByteCount(chars.Length);
            Console.WriteLine("Maximum number of bytes required: {0}\n",
                              maxBytes);

            // Encode the array of chars.
            byte[] utf8Bytes = utf8.GetBytes(chars);

            // Display all the UTF-8-encoded bytes.
            Console.WriteLine("UTF-8-encoded code units:");
            foreach (var utf8Byte in utf8Bytes)
                Console.Write("{0:X2} ", utf8Byte);
            Console.WriteLine();

            char[] asciiChars = new char[ascii.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
            ascii.GetChars(utf8Bytes, 0, utf8Bytes.Length, asciiChars, 0);
            return new string(asciiChars);
        }

        static string UnicodeToUTF8(string from)
        {
            byte[] bytes = Encoding.Default.GetBytes(from);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }
        public static void InsertRow(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionStringTabularValues))
            {

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception ex)

                    {
                        Console.WriteLine("error in InsertRow" + ex.Message);
                        Console.WriteLine("query: "+query);
                    }
                }
            }

        }
        public static DataTable GetDataTable(string query)
        {

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionStringPlace_new))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(query, connection))
                {
                    try
                    {
                        connection.Open();
                        da.Fill(dt);
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                }
            }

            return dt;
        }
        public class DataObject
        {
            public string Name { get; set; }
        }
        public class WikiObj
        {
            public string Title { get; set; }
            public string BOOK_ID { get; set; }
            public string Lang { get; set; }

        }
    }
}
//CREATE TABLE[dbo].[PLACE_WIKI_TEST_4_1_23]
//(
//    [Summary][nvarchar](MAX) NULL,
//  [Title] [nvarchar] (4000) NULL,
//	[BOOK_ID] [varchar] (20) NULL,
//	[LANG] [varchar] (20) NULL,
//	[Date] date DEFAULT GETDATE()
//) ON[PRIMARY]