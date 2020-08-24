using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources.NetStandard;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using FileScanApp;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Serilog;


namespace FileScanApp
{
    class HtmlValidator
    {
        private string[] allfiles { get; set; }
       private JObject ResourceObject { get; set; }
       private FileInfo FileInfo { get; set; }
       private string ResxFile { get; set; }
       private string DestinationFolder { get; set; } 

        public  void SearchFolder ()
        {
            try
            {
                allfiles = Directory.GetFiles(inputs(), "*.resx*", SearchOption.AllDirectories);
                if (allfiles.Length > 0)
                {
                    foreach (var file in allfiles)
                    {
                        ResourceObject = new JObject();
                        FileInfo = new FileInfo(file);
                        ResxFile = @"" + FileInfo + "";
                        using (ResXResourceReader resxReader = new ResXResourceReader(ResxFile))
                        {
                            foreach (DictionaryEntry entry in resxReader)
                            {
                                if (!HtmlScan(entry.Value.ToString()))
                                    if (!HtmlValidate(entry.Value.ToString()))
                                    {
                                        Log.Information("Html Not well Formated File:" + FileInfo.ToString() + " Key: " + entry.Key + " Value: " + entry.Value + "");
                                    }
                                ResourceObject.Add(new JProperty(entry.Key.ToString(), entry.Value.ToString()));
                            }
                        }
                        CreatejsonFile((Path.GetFileNameWithoutExtension(FileInfo.Name).Contains('.')) ? Path.GetFileNameWithoutExtension(FileInfo.Name).Substring(0, Path.GetFileNameWithoutExtension(FileInfo.Name).IndexOf(".") + 1).Replace(".", "") + "_" + GetLocale(FileInfo.FullName) : Path.GetFileNameWithoutExtension(FileInfo.Name));
                    }
                }
                else
                    Console.Out.WriteLine("No Resource Files (.resx) Found");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                 //throw new ArgumentException(ex.ToString());
                Console.Out.WriteLine(ex.ToString());
            }
        }

        private static bool HtmlScan(string resourcevalue)
        {
            if (resourcevalue != null)
            {
                try
                {
                    string regexpattern = @"(^((?!\<(|\/)[a-z][a-z0-9]*>).)*$)";
                    Regex htmlValidator = new Regex(regexpattern, RegexOptions.Compiled);
                    return htmlValidator.IsMatch(resourcevalue);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    throw new ArgumentException(ex.ToString());
                }
            }
            else
            {
                Exception ex = new Exception();
                Log.Error(ex, "Exception thrown on HtmlScan resourcevalue is null ");
                throw new ArgumentException(ex.ToString());
            }
        }

        private static bool HtmlValidate(string resourcevalue)
        {
            if (resourcevalue != null)
            {
                try
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(resourcevalue);
                    var parseErrors = htmlDocument.ParseErrors;
                    return !parseErrors.Any();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    throw new ArgumentException(ex.ToString());
                }
            }
            else
            {
                Exception ex = new Exception();
                Log.Error(ex, "Exception thrown on HtmlValidate : resourcevalue is null ");
                throw new ArgumentException(ex.ToString());
            }
        }

        private void CreatejsonFile(string Filename)
        {
            if (Filename != null)
            {
                try
                {
                    using (StreamWriter file = File.CreateText(@"../../../JsonFiles/" + Filename + ".json"))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        ResourceObject.WriteTo(writer);
                        Console.WriteLine("Json OutPut File Created " + Filename + ".json");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    throw new ArgumentException(ex.ToString());
                }
            }
            else
            {
                Exception ex = new Exception();
                Log.Error(ex, "Exception thrown on CreatejsonFile Filename is null ");
                throw new ArgumentException(ex.ToString());
            }
        }

        private static string GetLocale(string Filename)
        {
            if (Filename != null)
            {
                try
                {
                    string locale = string.Empty;
                    int pos;
                    if ((pos = Filename.IndexOf(".")) != -1)
                        locale = Filename.Substring(pos + 1);
                    if ((pos = locale.IndexOf(".")) != -1)
                        locale = locale.Substring(0, pos);

                    return locale;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    throw new ArgumentException(ex.ToString());
                }
            }
            else
            {
                Exception ex = new Exception();
                Log.Error(ex, "Exception thrown on GetLocale Filename is null ");
                throw new ArgumentException(ex.ToString());
            }
        }

        private string inputs ()
        {
            Console.Write("Please Enter your local Destination Folder to scan: ");


            DestinationFolder = Console.ReadLine();
            while (string.IsNullOrEmpty(DestinationFolder))
            {
                if (string.IsNullOrEmpty(DestinationFolder))
                {
                    Console.WriteLine("Destination Folder cant be empty! Enter Destination once more");
                    DestinationFolder = Console.ReadLine();
                }
            }
            if (!Directory.Exists(DestinationFolder))
            {
                Console.Out.WriteLine("Destination Folder Cant Be found");
                inputs();
               
            }
                return DestinationFolder;

        }
    }
}
