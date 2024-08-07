using Project.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interfaces;
public interface IRequestBarangRepository
{
    public List<BarangRequest> GetAllRequest();
    public void InsertRequest(BarangRequest request);
    public List<MasterBarang> GetAllBarang();
    BarangRequest GetRequestById(int id);
    public void UpdateRequest(BarangRequest request);
    public List<BarangRequest> GetAllReport(string? status);
    public List<MasterBarang> GetDataChart();
    public void AddToDatabase(List<MasterBarang> masterBarangs);

}

