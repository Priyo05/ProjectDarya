using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Project.Presentation.Web.Models;
using System.Data;

namespace Project.Presentation.Web.Controllers;
public class MasterBarangController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public MasterBarangController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("TestConnection");
    }


    [HttpGet]
    public IActionResult Index()
    {
        List<MasterBarangViewModel> Masterbarang = new List<MasterBarangViewModel>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SpSelectAllBarang", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MasterBarangViewModel barang = new MasterBarangViewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("KodeBarang")),
                            NamaBarang = reader.GetString(reader.GetOrdinal("NamaBarang")),
                            NamaSupplier = reader.GetString(reader.GetOrdinal("NamaSupplier")),
                            Harga = (int)reader.GetDecimal(reader.GetOrdinal("Harga")),
                            StokBarang = reader.GetInt32(reader.GetOrdinal("StokBarang"))
                        };
                        Masterbarang.Add(barang);
                    }
                }
            }
        }

        BarangViewModel barangModel = new BarangViewModel
        {
            Barang = Masterbarang
        };

        return View(barangModel);
    }


    [HttpGet]
    public IActionResult Insert()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Insert(MasterBarangViewModel masterBarangViewModel)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SpInsertBarang", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NamaBarang", masterBarangViewModel.NamaBarang);
                cmd.Parameters.AddWithValue("@NamaSupplier", masterBarangViewModel.NamaSupplier);
                cmd.Parameters.AddWithValue("@Harga", masterBarangViewModel.Harga);
                cmd.Parameters.AddWithValue("@StokBarang", masterBarangViewModel.StokBarang);

                cmd.ExecuteNonQuery();
            }
        }
        return RedirectToAction("Index");

    }

    [HttpGet("Update/{id}")]
    public IActionResult Update(long id)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SpSelectBarangByKode", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@KodeBarang", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        int Id = reader.GetInt32(reader.GetOrdinal("KodeBarang"));
                        string NamaBarang = reader.GetString(reader.GetOrdinal("NamaBarang"));
                        string NamaSupplier = reader.GetString(reader.GetOrdinal("NamaSupplier"));
                        int Harga = (int)reader.GetDecimal(reader.GetOrdinal("Harga"));
                        int StokBarang = reader.GetInt32(reader.GetOrdinal("StokBarang"));

                        MasterBarangViewModel barangModel = new MasterBarangViewModel
                        {
                            Id = Id,
                            NamaBarang = NamaBarang,
                            NamaSupplier = NamaSupplier,
                            Harga = Harga,
                            StokBarang = StokBarang,
                        };

                        return View("Update", barangModel);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }
    }

    [HttpPost("Update/{id}")]
    public IActionResult Update(int id, string namaBarang, string namaSupplier, int harga,int stokBarang)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SpUpdateBarang", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@KodeBarang", id);
                cmd.Parameters.AddWithValue("@NamaBarang", namaBarang);
                cmd.Parameters.AddWithValue("@NamaSupplier", namaSupplier);
                cmd.Parameters.AddWithValue("@Harga", harga);
                cmd.Parameters.AddWithValue("@StokBarang", stokBarang);

                cmd.ExecuteNonQuery();
            }
        }
        return RedirectToAction("Index");
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand("SpDeleteBarang", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@KodeBarang", id);
                cmd.ExecuteNonQuery();
            }
        }
        return RedirectToAction("Index");
    }


}

