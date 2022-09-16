using MergeHelper;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MergerUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            StartMerging();
        }

        private static void StartMerging()
        {
            try
            {
                string configuration = File.ReadAllText($@"{AppDomain.CurrentDomain.BaseDirectory}\\config.json");
                var config = JsonConvert.DeserializeObject<Configuration>(configuration);
                Console.WriteLine("Merge Started");
                if (!string.IsNullOrEmpty(config.FileDirectory))
                {
                    var docxFiles = Directory.GetFiles(config.FileDirectory, "*.docx");
                    if (!string.IsNullOrEmpty(config.MergeFileName))
                    {
                        IHelper helper = new Helper();
                        helper.MergeDocument(config.NewFilePath, docxFiles);
                        Console.WriteLine("Merge End");
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Unable to Merge Documents");
                Console.WriteLine(ex.StackTrace);

            }
        }
    }
}
