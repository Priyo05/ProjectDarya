using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Business.Interfaces;
using Project.DataAccess.Models;
using Project.Presentation.Web.Models;



namespace Project.Presentation.Web.Services;
public class RequestBarangService
{
    private readonly IRequestBarangRepository _requestBarangRepository;

    public RequestBarangService(IRequestBarangRepository requestBarangRepository)
    {
        _requestBarangRepository = requestBarangRepository;
    }

    public RequestBarangIndexViewModel GetAllRequest()
    {
        List<RequestBarangViewModel> result;

        result = _requestBarangRepository.GetAllRequest()
            .Select(c => new RequestBarangViewModel
            {
                Id = c.Id,
                NamaDivisi = c.NamaDivisi,
                KodeBarang = c.KodeBarang,
                NamaBarang = c.NamaBarang,
                Jumlah = c.Jumlah,
                Status = c.Status,
                RequestDate = c.RequestDate
            })
            .ToList();

        return new RequestBarangIndexViewModel
        {
            RequestBarang = result
        };

    }

    public List<MasterBarangViewModel> GetDataChart()
    {

        List<MasterBarangViewModel> result;

        result = _requestBarangRepository.GetDataChart()
            .Select(c => new MasterBarangViewModel
            {
                NamaBarang = c.NamaBarang,
                StokBarang = (int)c.StokBarang
            })
            .ToList();

        return result;

    }

    public void InsertRequest(RequestBarangViewModel viewModel)
    {
        BarangRequest result = new BarangRequest
        {
            NamaDivisi = viewModel.NamaDivisi,
            KodeBarang = viewModel.KodeBarang,
            Jumlah = viewModel.Jumlah,
            Status = "Pending",
            RequestDate = DateTime.Now,
        };

        _requestBarangRepository.InsertRequest(result);
    }


    public List<SelectListItem> GetBarang()
    {
        var barang = _requestBarangRepository.GetAllBarang();

        var selectListItem = barang.OrderBy(c => c.NamaBarang)
            .Select(c => new SelectListItem
            {
                Value = c.KodeBarang.ToString(),
                Text = c.NamaBarang+" - "+ c.StokBarang+" Unit",
            })
            .ToList();


        return selectListItem;
    }


    public void ApproveRequest(int id)
    {
        var request = _requestBarangRepository.GetRequestById(id);

        request.Status = "Approved";
        _requestBarangRepository.UpdateRequest(request);
    }

    public void RejectRequest(int id)
    {
        var request = _requestBarangRepository.GetRequestById(id);

        request.Status = "Rejected";
        _requestBarangRepository.UpdateRequest(request);
    }


    public ReportBarangIndexViewModel GetAllReport(string? status)
    {
        List<RequestBarangViewModel> result;

        result = _requestBarangRepository.GetAllReport(status)
            .Select(c => new RequestBarangViewModel
            {
                Id = c.Id,
                NamaDivisi = c.NamaDivisi,
                KodeBarang = c.KodeBarang,
                NamaBarang = c.NamaBarang,
                Jumlah = c.Jumlah,
                Status = c.Status,
                RequestDate = c.RequestDate
            })
            .ToList();

        return new ReportBarangIndexViewModel
        {
            ReportBarangViewModels = result,
            Status = status
        };

    }

    public void AddToDatabase(List<MasterBarangViewModel> viewModels)
    {
        var masterBarangEntity = viewModels.Select( c => new MasterBarang
        {
            NamaBarang = c.NamaBarang,
            NamaSupplier = c.NamaSupplier,
            Harga = c.Harga,
            StokBarang = c.StokBarang,
        }).ToList();

        _requestBarangRepository.AddToDatabase(masterBarangEntity);
    }

    public PDFBarangViewModel GetBarangPDF(string? status,DateTime? fromDate, DateTime? toDate)
    {
        List<RequestBarangViewModel> resutl;

        resutl = _requestBarangRepository.GetPDFBarang(status, fromDate, toDate)
            .Select(c => new RequestBarangViewModel
            {
                KodeBarang = c.KodeBarang,
                NamaBarang = c.NamaBarang,
                Jumlah = c.Jumlah,
                Status = c.Status,
                NamaDivisi = c.NamaDivisi,
                RequestDate = c.RequestDate,
            }).ToList();


        return new PDFBarangViewModel
        {
            Bars = resutl,
            Status = status,
            FromDate = fromDate,
            ToDate = toDate,
        };
    }









}

