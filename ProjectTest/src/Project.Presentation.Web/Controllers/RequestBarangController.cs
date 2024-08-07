using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Project.Presentation.Web.Models;
using Project.Presentation.Web.Services;

namespace Project.Presentation.Web.Controllers;

[Route("Request")]
public class RequestBarangController : Controller
{

    private readonly RequestBarangService _service;

    public RequestBarangController(RequestBarangService service)
    {
        _service = service;
    }


    [HttpGet()]
    public IActionResult Index()
    {

        var vm = _service.GetAllRequest();
        return View(vm);
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

        } catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    
    }

}

