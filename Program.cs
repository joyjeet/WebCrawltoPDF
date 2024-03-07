using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using HtmlAgilityPack;
using WebCrawltoPDF;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string url = "https://leidos.widen.net/s/gcrlxhlfkj/23-836952-esg_index-digital"; // Replace with the URL of the webpage you want to scrape
        //string url = "https://www.leidos.com/sites/leidos/files/2022-03/REPORT-CR-Leidos-2020.pdf"; // Replace with the URL of the webpage you want to scrape
        string fileName = url.Substring(url.LastIndexOf('/') + 1);
        try
        {
            if (fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower() == "pdf")
            {
                DownloadFileAsync(url, fileName).GetAwaiter().GetResult();


            }
            else
            {

                // Fetch HTML content
                FetchURLContent(url, fileName);



            }
            // Parse HTML and print all hyperlinks
            //ParseHtmlAndPrintHyperlinks(htmlContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to fetch HTML content or parse HTML: {ex.Message}");
        }
    }

    private static void CreateHTMLFile(string htmlContent, string fileName)
    {
        
        using (var writer = new StreamWriter(fileName))

            //save in html file
            writer.Write(htmlContent);

        
    }

    private static async Task DownloadFileAsync(string url, string outputPath)
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    using (Stream streamToWriteTo = File.Open(outputPath, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }
                }
            }
        }
    }

    static void FetchURLContent(string url, string fileName)
    {
        
        var web = new HtmlWeb();
        // downloading to the target page
        // and parsing its HTML content
        var document = web.Load(url);
        var nodes = document.DocumentNode.SelectNodes("//html");

        if (nodes[0] != null && Regex.IsMatch(nodes[0].InnerHtml, @"\s*pdf\s*", RegexOptions.IgnoreCase))
        {
            var scriptNodes = document.DocumentNode.SelectNodes("//a[@href]");
            foreach (var node in scriptNodes)
            {
                string pdfUrl = node.GetAttributeValue("href", "");

                if (Regex.IsMatch(pdfUrl, @"\bpdf\b", RegexOptions.IgnoreCase))
                {
                    //string pdfUrl = node.InnerHtml.Substring(node.InnerHtml.IndexOf("https"), node.InnerHtml.IndexOf("pdf") - node.InnerHtml.IndexOf("https") + 3);

                    Uri uri = new Uri(url);
                    string domain = uri.Host;
                    Console.WriteLine(domain);
                    
                    if (pdfUrl.Contains(domain) == false)
                    {
                        pdfUrl = uri.Scheme + "://" + domain + pdfUrl;
                    }

                    Console.WriteLine(pdfUrl);

                    if(fileName.Contains(".pdf") == false)
                    {
                        fileName = fileName + ".pdf";
                    }
                    Console.WriteLine(pdfUrl);
                    DownloadFileAsync(pdfUrl, fileName).GetAwaiter().GetResult();
                }
            }
        }
        else
        {
            string htmlContent = "";
            foreach (var node in nodes)
            {


                htmlContent = htmlContent + node.InnerHtml;

                Console.WriteLine(node.InnerHtml);
            }
            //save in file
            CreateHTMLFile(htmlContent, fileName);

        }
    }

    static void ParseHtmlAndPrintHyperlinks(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
        {
            HtmlAttribute att = link.Attributes["href"];
            Console.WriteLine(att.Value);
        }
    }

    public static byte[] ConvertHtmlToPdf(string html)
    {
        byte[] result = null;
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
        //    pdf.Save(ms);
        //    result = ms.ToArray();
        //}
        return result;
    }
}
