using System.ComponentModel.DataAnnotations;

namespace Project.Presentation.Web.Models;
public class PDFBarangViewModel
{
    public List<RequestBarangViewModel>? Bars { get; set; }
    public string? Status { get; set; }
    [Required]
    public DateTime? FromDate { get; set; }
    [Required]
    public DateTime? ToDate{ get; set; }
}

