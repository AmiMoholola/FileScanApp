using HtmlAgilityPack;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using FileScanApp;
using System.Resources.NetStandard;
using Serilog;


namespace FileScanApp
{
 
   class Program
    {
        static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("../../../logs/FileScanApp.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            FileScanApp.HtmlValidator HtmlValidate = new HtmlValidator();

            HtmlValidate.SearchFolder();
        }
    }
}
