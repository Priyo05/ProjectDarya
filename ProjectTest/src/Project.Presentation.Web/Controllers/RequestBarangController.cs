using ExcelDataReader;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Project.Presentation.Web.Models;
using Project.Presentation.Web.Services;
using System.IO;

namespace Project.Presentation.Web.Controllers;

[Route("Request")]
public class RequestBarangController : Controller
{

    private readonly RequestBarangService _service;
    private readonly IMemoryCache _memoryCache;

    public RequestBarangController(RequestBarangService service, IMemoryCache memoryCache)
    {
        _service = service;
        _memoryCache = memoryCache;
    }

    [HttpGet()]
    public IActionResult Index()
    {
        string cacheRequest = "Request";
        if (!_memoryCache.TryGetValue(cacheRequest, out RequestBarangIndexViewModel requestBarang))
        {
            requestBarang = _service.GetAllRequest();
            var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set(cacheRequest, requestBarang, cacheOptions);
        }
        return View(requestBarang);
    }


    [HttpGet("Insert")]
    public IActionResult Insert()
    {
        return View("Insert", new RequestBarangViewModel
        {
            Barang = _service.GetBarang()
        });
    }

    [HttpPost("Insert")]
    public IActionResult Insert(RequestBarangViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                viewModel.Barang = _service.GetBarang();
                return View("Insert", viewModel);
            }

            _service.InsertRequest(viewModel);

            string cacheRequest = "Request";
            _memoryCache.Remove(cacheRequest);

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Insert", viewModel);
        }
    }

    [HttpGet("Approved/{id}")]
    public IActionResult Approved(int id)
    {
        try
        {
            _service.ApproveRequest(id);
            string cacheRequest = "Request";
            _memoryCache.Remove(cacheRequest);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpGet("Reject/{id}")]
    public IActionResult Reject(int id)
    {
        try
        {
            _service.RejectRequest(id);
            string cacheRequest = "Request";
            _memoryCache.Remove(cacheRequest);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpGet("Report")]
    public IActionResult Report(string? status)
    {

        var dataChart = _service.GetDataChart();

        ViewBag.Data = dataChart;

        var vm = _service.GetAllReport(status);
        return View(vm);
    }

    [HttpGet("ExcelReader")]
    public IActionResult ExcelReader()
    {
        return View();
    }

    [HttpPost("ExcelReader")]
    public async Task<IActionResult> ExcelReader(IFormFile formFile)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        //upload file
        if (formFile != null && formFile.Length > 0)
        {
            var uploadDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\uploads";

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var filePath = Path.Combine(uploadDirectory, formFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            //read file
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                var excelData = new List<List<object>>();
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            var rowData = new List<object>();
                            for (int column = 0; column < reader.FieldCount; column++)
                            {
                                rowData.Add(reader.GetValue(column));
                            }
                            excelData.Add(rowData);
                        }
                    } while (reader.NextResult());

                    ViewBag.excelData = excelData;

                }
            }

        }

        return View();
    }


    [HttpPost("AddToDatabase")]
    public async Task<IActionResult> AddToDatabase()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var directory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\uploads";

        if (!Directory.Exists(directory))
        {
            return BadRequest("Direktori tidak ada");
        };

        var directoryInfo = new DirectoryInfo(directory);
        var latestFile = directoryInfo.GetFiles()
                                      .OrderByDescending(f => f.LastWriteTime)
                                      .FirstOrDefault();

        if (latestFile == null)
        {
            return BadRequest("No files found in directory.");
        }

        var masterBarangList = new List<MasterBarangViewModel>();
        try
        {

            using (var stream = System.IO.File.Open(latestFile.FullName, FileMode.Open, FileAccess.Read))
            {

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        bool isHeaderSkipped = false;
                        while (reader.Read())
                        {
                            if (!isHeaderSkipped)
                            {
                                isHeaderSkipped = true;
                                continue;
                            }

                            var masterBarang = new MasterBarangViewModel
                            {
                                NamaBarang = reader.GetValue(0)?.ToString(),
                                NamaSupplier = reader.GetValue(1)?.ToString(),
                                Harga = reader.GetValue(2) != null ? Convert.ToInt32(reader.GetValue(2).ToString()) : 0,
                                StokBarang = reader.GetValue(3) != null ? Convert.ToInt32(reader.GetValue(3).ToString()) : 0
                            };

                            masterBarangList.Add(masterBarang);
                        }
                    } while (reader.NextResult());
                }
            }

            _service.AddToDatabase(masterBarangList);


            return View("ExcelReader");

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpGet("FilterPDF")]
    public IActionResult FilterPDF(PDFBarangViewModel pDFBarangViewModel)
    {
        try
        {
            var getResult = _service.GetBarangPDF(pDFBarangViewModel.Status, pDFBarangViewModel.FromDate, pDFBarangViewModel.ToDate);
            string guidToken = Guid.NewGuid().ToString();
            string jsonData = JsonConvert.SerializeObject(getResult);
            HttpContext.Session.SetString("FilteredData" + guidToken, jsonData);

            ViewBag.DataToken = guidToken;

            return View(getResult);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("CreatePDF")]
    public IActionResult CreatePDF(string dataToken)
    {
        // Ambil data yang tersimpan di session
        string serializedData = HttpContext.Session.GetString("FilteredData" + dataToken);

        // Cek apakah data ada di session
        if (string.IsNullOrEmpty(serializedData))
        {
            Console.WriteLine("Data tidak ada");
            return BadRequest("Data tidak ditemukan atau session kadaluarsa.");
        }

        // Deserialisasi data dari JSON ke objek PDFBarangViewModel
        var pDFBarangViewModel = JsonConvert.DeserializeObject<PDFBarangViewModel>(serializedData);

        if (pDFBarangViewModel == null || pDFBarangViewModel.Bars == null || !pDFBarangViewModel.Bars.Any())
        {
            Console.WriteLine("Tidak ada data untuk ditampilkan");
            return BadRequest("Tidak ada data yang bisa diproses.");
        }

        var pdfStream = GenerateReportPdf(pDFBarangViewModel); // ini di service

        var fromDate = pDFBarangViewModel.FromDate.Value;
        var toDate = pDFBarangViewModel.ToDate.Value;


        return File(pdfStream, "application/pdf", $"report/{pDFBarangViewModel.Status}/{fromDate:dd/MM/yyyy}-to-{toDate:dd/MM/yyyy}.pdf");
    }


    private static byte[] GenerateReportPdf(PDFBarangViewModel viewModel)
    {

        var fromDate = viewModel.FromDate.Value; // Ambil nilai dari nullable
        var toDate = viewModel.ToDate.Value;     // Ambil nilai dari nullable

        using var memoryStream = new MemoryStream();
        using (var pdfWriter = new PdfWriter(memoryStream))
        using (var pdf = new PdfDocument(pdfWriter))
        using (var document = new Document(pdf))
        {
            document.Add(new Paragraph("Laporan Barang").SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph($"Status: {viewModel.Status}").SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph($"Dari Tanggal: {fromDate:dd/MM/yyyy}").SetTextAlignment(TextAlignment.LEFT));
            document.Add(new Paragraph($"Sampai Tanggal: {toDate:dd/MM/yyyy}").SetTextAlignment(TextAlignment.LEFT));

            // Buat tabel
            var table = new Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth();
            table.AddHeaderCell("Nama Divisi");
            table.AddHeaderCell("Kode Barang");
            table.AddHeaderCell("Nama Barang");
            table.AddHeaderCell("Jumlah");
            table.AddHeaderCell("Status");
            table.AddHeaderCell("Tanggal Request");

            foreach (var item in viewModel.Bars)
            {

                table.AddCell(item.NamaDivisi);
                table.AddCell(item.KodeBarang.ToString());
                table.AddCell(item.NamaBarang);
                table.AddCell(item.Jumlah.ToString());
                table.AddCell(item.Status);
                table.AddCell(item.RequestDate.ToString());
            }

            document.Add(table);
            document.Close(); // Tutup document
        }

        return memoryStream.ToArray(); // Kembalikan byte array dari MemoryStream
    }

}

