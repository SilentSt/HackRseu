using System;
using System.Net.Http;
using System.IO;
using FastReport.Cloud;
using System.Threading.Tasks;
using FastReport.Cloud.ReportProcessor;
using System.Collections.Generic;
using System.Linq;
using FastReport.Cloud.ResultsProvider;
using System.Threading;
using FastReport.Cloud.Management;
using FsCheck;
using System.Text;
using Telegram.Bot.Types.InputFiles;

namespace rseuHack
{
    public class Authorization
    {
        static Authorization auth;
        private Authorization() { }
        private static HttpClient client = new HttpClient();
        static SubscriptionVM subscription;
        static HttpClient httpClient;

        public static Dictionary<string, ExportTemplateTaskVMFormat> formats = new Dictionary<string, ExportTemplateTaskVMFormat>();

        public static Authorization GetAuthorization()
        {
            if (auth == null)
            {
                auth = new Authorization();
                formats.Add("pdf", ExportTemplateTaskVMFormat.Pdf);
                formats.Add("rtf", ExportTemplateTaskVMFormat.Richtext);
                formats.Add("docx", ExportTemplateTaskVMFormat.Docx);
                formats.Add("svg", ExportTemplateTaskVMFormat.Svg);
                formats.Add("csv", ExportTemplateTaskVMFormat.Csv);
                formats.Add("ps", ExportTemplateTaskVMFormat.PS);
                formats.Add("pptx", ExportTemplateTaskVMFormat.Pptx);
                formats.Add("json", ExportTemplateTaskVMFormat.Json);
                formats.Add("dbf", ExportTemplateTaskVMFormat.Dbf);
                formats.Add("html", ExportTemplateTaskVMFormat.Html);
                formats.Add("img", ExportTemplateTaskVMFormat.Image);
                formats.Add("ods", ExportTemplateTaskVMFormat.Ods);
                formats.Add("odt", ExportTemplateTaskVMFormat.Odt);
                formats.Add("zpl", ExportTemplateTaskVMFormat.Zpl);
                formats.Add("xaml", ExportTemplateTaskVMFormat.Xaml);
                formats.Add("xml", ExportTemplateTaskVMFormat.Xml);
                auth.GetSubscription();  
            }
            return auth;
        }

        public async Task Magic(string filepath,string fileType, long? userId)
        {
            var fType = fileType.ToLower();
            var rpClientTemplates = new TemplatesClient(httpClient);
            var rpClientExports = new ExportsClient(httpClient);
            var downloadClient = new DownloadClient(httpClient);

            var templateFolder = subscription.TemplatesFolder.FolderId;
            var exportFolder = subscription.ExportsFolder.FolderId;

            TemplateCreateVM templateCreateVM = new TemplateCreateVM()
            {
                Name = "box.frx",
                Content = Convert.ToBase64String(File.ReadAllBytes(filepath))
            };

            TemplateVM uploadedFile = await rpClientTemplates.UploadFileAsync(templateFolder, templateCreateVM);

            ExportTemplateTaskVM export = new ExportTemplateTaskVM()
            {
                FileName = "box."+fileType,
                FolderId = exportFolder,
                Format = formats[fileType]
            };
            ExportVM exportedFile = await rpClientTemplates.ExportAsync(uploadedFile.Id, export) as ExportVM;
            string fileId = exportedFile.Id;
            int attempts = 3;

            exportedFile = rpClientExports.GetFile(fileId);
            while (exportedFile.Status != ExportVMStatus.Success && attempts >= 0)
            {
                await Task.Delay(1000);
                exportedFile = rpClientExports.GetFile(fileId);
                attempts--;
            }

            using (var file = await downloadClient.GetExportAsync(fileId))
            {
                using (var pdf = File.Open("report."+fileType, FileMode.Create))
                {
                    file.Stream.CopyTo(pdf);
                }
                using (var stream = File.Open("report." + fileType, FileMode.Open))
                {
                    await Program.tgBot.SendDocumentAsync(userId, new InputOnlineFile(stream, "report." + fileType));
                }
            }
        }

        public async void GetSubscription()
        {
            var str = File.ReadAllText("scp682");
            client.DefaultRequestHeaders.Authorization = new FastReportCloudApiKeyHeader(str);
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://fastreport.cloud");
            httpClient.DefaultRequestHeaders.Authorization = new FastReportCloudApiKeyHeader(str);
            var subscriptions = new SubscriptionsClient(httpClient);
            subscription = (await subscriptions.GetSubscriptionsAsync(0, 2)).Subscriptions.First();
        }        
    }
}
