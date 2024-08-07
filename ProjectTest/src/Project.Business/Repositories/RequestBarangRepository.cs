using Microsoft.EntityFrameworkCore;
using Project.Business.Interfaces;
using Project.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Repositories;
public class RequestBarangRepository : IRequestBarangRepository
{

    private readonly ProjectContext _projectContext;

    public RequestBarangRepository(ProjectContext projectContext)
    {
        _projectContext = projectContext;
    }

    public List<BarangRequest> GetAllRequest()
    {
        var query = _projectContext.BarangRequests
                              .FromSqlRaw("EXEC SpGetPendingRequests");

        return query.ToList();
    }


    public void InsertRequest(BarangRequest request)
    {
        var barang = _projectContext.MasterBarangs
        .FirstOrDefault(b => b.KodeBarang == request.KodeBarang);

        if(barang == null)
        {
            throw new Exception("barang tidak ditemukan atau tidak ada");
        }


        if(request.Jumlah <= barang.StokBarang)
        {
            request.NamaBarang = barang.NamaBarang;
            barang.StokBarang = barang.StokBarang - request.Jumlah;

            _projectContext.BarangRequests.Add(request);
            _projectContext.SaveChanges();
        }
        else
        {
            throw new Exception("Jumlah barang melebihi Stok barang");
        }

    }

    public List<MasterBarang> GetAllBarang()
    {
        return _projectContext.MasterBarangs.ToList();
    }

    public BarangRequest GetRequestById(int id)
    {
        return _projectContext.BarangRequests.FirstOrDefault(r => r.Id == id)??
            throw new Exception("Request tidak ditemukan");
    }

    public void UpdateRequest(BarangRequest request)
    {
        if(request.Status == "Approved")
        {
            _projectContext.BarangRequests.Update(request);
        } else if ( request.Status == "Rejected")
        {
            var barang = _projectContext.MasterBarangs
                .FirstOrDefault(b => b.KodeBarang == request.KodeBarang);

            barang.StokBarang = barang.StokBarang + request.Jumlah;
        }

        _projectContext.SaveChanges();
    }


    public List<BarangRequest> GetAllReport(string? status)
    {
        var query = from request in _projectContext.BarangRequests
                    select request;

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(request => request.Status == status);
        }

        return query.ToList();
    }


    public List<MasterBarang> GetDataChart()
    {
        var query = from data in _projectContext.MasterBarangs
                    select data;

        return query.ToList();
    }

    public void AddToDatabase(List<MasterBarang> masterBarangs)
    {

        foreach(var item in masterBarangs)
        {
            var dbItem = _projectContext.MasterBarangs.FirstOrDefault(
                c => c.NamaBarang.ToLower() == item.NamaBarang.ToLower() &&
                c.NamaSupplier.ToLower() == item.NamaSupplier.ToLower());

            if(dbItem != null)
            {
                dbItem.StokBarang = dbItem.StokBarang + item.StokBarang;
            }
            else
            {

                MasterBarang master = new MasterBarang
                {
                    NamaBarang = item.NamaBarang,
                    NamaSupplier = item.NamaSupplier,
                    Harga = item.Harga,
                    StokBarang = item.StokBarang
                };
                _projectContext.MasterBarangs.Add(master);

            }
        }
        
        _projectContext.SaveChanges();

    }


}

