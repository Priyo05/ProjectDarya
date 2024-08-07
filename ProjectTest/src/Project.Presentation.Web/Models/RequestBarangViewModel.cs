using Microsoft.AspNetCore.Mvc.Rendering;

namespace Project.Presentation.Web.Models;
public class RequestBarangViewModel
{
    public int Id { get; set; }
    public string NamaDivisi { get; set; }
    public int KodeBarang { get; set; }
    public string? NamaBarang { get; set; }
    public int Jumlah { get; set; }
    public string? Status { get; set; }
    public DateTime? RequestDate { get; set; }
    public List<SelectListItem>? Barang {  get; set; }
}

