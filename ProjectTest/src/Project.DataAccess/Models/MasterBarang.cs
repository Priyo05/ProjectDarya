using System;
using System.Collections.Generic;

namespace Project.DataAccess.Models;

public partial class MasterBarang
{
    public int KodeBarang { get; set; }

    public string NamaBarang { get; set; } = null!;

    public string NamaSupplier { get; set; } = null!;

    public decimal? Harga { get; set; }

    public int? StokBarang { get; set; }

    public virtual ICollection<BarangRequest> BarangRequests { get; set; } = new List<BarangRequest>();
}
