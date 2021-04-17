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
        public static Authorization GetAuthorization()
        {
            if (auth == null)
            {
                auth = new Authorization();
                auth.GetSubscription();
            }
            return auth;
        }

        public async Task PDF(string filepath, long? userId)
        {
            
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
                FileName = "box.pdf",
                FolderId = exportFolder,
                Format = ExportTemplateTaskVMFormat.Pdf
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
                using (var pdf = File.Open("report.pdf", FileMode.Create))
                {
                    file.Stream.CopyTo(pdf);
                }
                using (var stream = File.Open("report.pdf", FileMode.Open))
                {
                    await Program.tgBot.SendDocumentAsync(userId, new InputOnlineFile(stream, "report.pdf"));
                }
            }
            //await Program.tgBot.SendDocumentAsync(userId, name);



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


        public async Task RTF(string filepath, long? userId)
        {

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
                FileName = "box.rtf",
                FolderId = exportFolder,
                Format = ExportTemplateTaskVMFormat.Richtext
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
                using (var pdf = File.Open("report.rtf", FileMode.Create))
                {
                    file.Stream.CopyTo(pdf);
                }
                using (var stream = File.Open("report.rtf", FileMode.Open))
                {
                    await Program.tgBot.SendDocumentAsync(userId, new InputOnlineFile(stream, "report.rtf"));
                }
            }
            //await Program.tgBot.SendDocumentAsync(userId, name);



        }
        public async Task DOCX(string filepath, long? userId)
        {

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
                FileName = "box.docx",
                FolderId = exportFolder,
                Format = ExportTemplateTaskVMFormat.Docx
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
                using (var pdf = File.Open("report.docx", FileMode.Create))
                {
                    file.Stream.CopyTo(pdf);
                }
                using (var stream = File.Open("report.docx", FileMode.Open))
                {
                    await Program.tgBot.SendDocumentAsync(userId, new InputOnlineFile(stream, "report.docx"));
                }
            }
            //await Program.tgBot.SendDocumentAsync(userId, name);



        }
        public async Task SVG(string filepath, long? userId)
        {
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
                FileName = "box.svg",
                FolderId = exportFolder,
                Format = ExportTemplateTaskVMFormat.Svg
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
                using (var pdf = File.Open("report.svg", FileMode.Create))
                {
                    file.Stream.CopyTo(pdf);
                }
                using (var stream = File.Open("report.svg", FileMode.Open))
                {
                    await Program.tgBot.SendDocumentAsync(userId, new InputOnlineFile(stream, "report.svg"));
                }
            }
            //await Program.tgBot.SendDocumentAsync(userId, name);



        }
    }
}
