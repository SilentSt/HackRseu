using FastReport.Cloud;
using FastReport.Cloud.ReportProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace rseuHack
{
    static class Converter
    {
        public static async Task<string> ExportReportPDF(HttpClient httpClient,
                                       string folderId,
                                       string reportId,
                                       string fileName)
        {
            IReportsClient reportsClient = new ReportsClient(httpClient);

            ExportReportTaskVM task = new ExportReportTaskVM()
            {
                FileName = Path.ChangeExtension(fileName, ".pdf"),
                FolderId = folderId,
                Format = ExportReportTaskVMFormat.Pdf
            };

            ExportVM result = await reportsClient.ExportAsync(reportId, task);

            return result.Id;
        }

        public static async Task<string> ExportReportRTF(HttpClient httpClient,
                                       string folderId,
                                       string reportId,
                                       string fileName)
        {
            IReportsClient reportsClient = new ReportsClient(httpClient);

            ExportReportTaskVM task = new ExportReportTaskVM()
            {
                FileName = Path.ChangeExtension(fileName, ".rtf"),
                FolderId = folderId,
                Format = ExportReportTaskVMFormat.Richtext
            };

            ExportVM result = await reportsClient.ExportAsync(reportId, task);

            return result.Id;
        }

        public static async Task<string> ExportReportDOCX(HttpClient httpClient,
                                       string folderId,
                                       string reportId,
                                       string fileName)
        {
            IReportsClient reportsClient = new ReportsClient(httpClient);

            ExportReportTaskVM task = new ExportReportTaskVM()
            {
                FileName = Path.ChangeExtension(fileName, ".docx"),
                FolderId = folderId,
                Format = ExportReportTaskVMFormat.Docx
            };

            ExportVM result = await reportsClient.ExportAsync(reportId, task);

            return result.Id;
        }

        public static async Task<string> ExportReportSVG(HttpClient httpClient,
                                       string folderId,
                                       string reportId,
                                       string fileName)
        {
            IReportsClient reportsClient = new ReportsClient(httpClient);

            ExportReportTaskVM task = new ExportReportTaskVM()
            {
                FileName = Path.ChangeExtension(fileName, ".svg"),
                FolderId = folderId,
                Format = ExportReportTaskVMFormat.Svg
            };

            ExportVM result = await reportsClient.ExportAsync(reportId, task);

            return result.Id;
        }

        public static async Task<string> ExportReportCSV(HttpClient httpClient,
                                       string folderId,
                                       string reportId,
                                       string fileName)
        {
            IReportsClient reportsClient = new ReportsClient(httpClient);

            ExportReportTaskVM task = new ExportReportTaskVM()
            {
                FileName = Path.ChangeExtension(fileName, ".csv"),
                FolderId = folderId,
                Format = ExportReportTaskVMFormat.Csv
            };

            ExportVM result = await reportsClient.ExportAsync(reportId, task);

            return result.Id;
        }

        public static async Task<string> ExportReportXLS(HttpClient httpClient,
                                       string folderId,
                                       string reportId,
                                       string fileName)
        {
            IReportsClient reportsClient = new ReportsClient(httpClient);

            ExportReportTaskVM task = new ExportReportTaskVM()
            {
                FileName = Path.ChangeExtension(fileName, ".xls"),
                FolderId = folderId,
                Format = ExportReportTaskVMFormat.Xlsx
            };

            ExportVM result = await reportsClient.ExportAsync(reportId, task);

            return result.Id;
        }
    }
}
