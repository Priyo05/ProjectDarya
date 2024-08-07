using System;
using System.Collections.Generic;

namespace Project.DataAccess.Models;

public partial class BarangRequest
{
    public int Id { get; set; }

    public string NamaDivisi { get; set; } = null!;

    public int KodeBarang { get; set; }

    public string NamaBarang { get; set; } = null!;

    public int Jumlah { get; set; }

    public string Status { get; set; } = null!;

    public DateTime RequestDate { get; set; }

    public virtual MasterBarang KodeBarangNavigation { get; set; } = null!;
}
