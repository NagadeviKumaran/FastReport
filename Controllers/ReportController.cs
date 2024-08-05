using FastReport;
using FastReport.Export.Html;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using System.Text;




namespace SalarySlipFR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly string reportPath = @"C:\Users\KUMARAN.K.B\FastReport\Reports\SalarySlip.frx";


        //[HttpGet]
        //public IActionResult ExportSalarySlipToPdf()
        //{
        //    // Create a report instance
        //    Report report = new Report();

        //    try
        //    {
        //        // Load the report template
        //        report.Load(reportPath);

        //        // If needed, register any data sources here
        //        // e.g., report.RegisterData(dataSet, "DataSet");

        //        // Prepare the report
        //        report.Prepare();

        //        // Export the report to PDF
        //        var pdfExport = new PDFSimpleExport();
        //        using var ms = new MemoryStream();
        //        pdfExport.Export(report, ms);

        //        // Ensure the stream position is reset to the beginning before returning
        //        ms.Position = 0;

        //        // Return the PDF file as a stream
        //        return File(ms.ToArray(), "application/pdf", "report.pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions
        //        return BadRequest(ex.Message);
        //    }
        //    finally
        //    {
        //        // Dispose of the report
        //        report.Dispose();
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> ExportSalarySlipToPdfAsync()
        {
            // Create a report instance
            Report report = new Report();

            try
            {
                // Load the report template
                report.Load(reportPath);

                // If needed, register any data sources here
                // e.g., report.RegisterData(dataSet, "DataSet");

                // Prepare the report
                report.Prepare();

                // Export the report to HTML
                var htmlExport = new HTMLExport
                {
                    EmbedPictures = true // Ensure images are embedded
                };

                using var htmlStream = new MemoryStream();
                htmlExport.Export(report, htmlStream);

                // Ensure the stream position is reset to the beginning before reading
                htmlStream.Position = 0;

                // Convert the HTML to a string
                string htmlContent;
                using (var reader = new StreamReader(htmlStream))
                {
                    htmlContent = await reader.ReadToEndAsync();
                }

               


                // Setup PuppeteerSharp to convert HTML to PDF
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();
                using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                using var page = await browser.NewPageAsync();

                // Set HTML content and convert to PDF
                await page.SetContentAsync(htmlContent);
                var pdfStream = await page.PdfStreamAsync();

                // Convert the PDF stream to a memory stream
                using var memoryStream = new MemoryStream();
                await pdfStream.CopyToAsync(memoryStream);

                // Ensure the stream position is reset to the beginning before returning
                memoryStream.Position = 0;

                // Return the PDF file as a stream
                return File(memoryStream.ToArray(), "application/pdf", "report.pdf");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return BadRequest(ex.Message);
            }
            finally
            {
                // Dispose of the report
                report.Dispose();
            }
        }

    }
}





    

