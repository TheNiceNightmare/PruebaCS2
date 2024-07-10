using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SegurosChubbi.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

public class SeguroController : Controller
{
    private readonly SqlDbHelper _dbHelper;

    public SeguroController(IConfiguration configuration)
    {
        _dbHelper = new SqlDbHelper(configuration);
    }

    public IActionResult Index()
    {
        string query = "SELECT * FROM Seguros";
        DataTable dt = _dbHelper.ExecuteQuery(query);
        List<Seguro> seguros = new List<Seguro>();

        foreach (DataRow row in dt.Rows)
        {
            seguros.Add(new Seguro
            {
                SeguroId = (int)row["SeguroId"],
                Nombre = row["Nombre"].ToString(),
                Codigo = row["Codigo"].ToString(),
                SumaAsegurada = (decimal)row["SumaAsegurada"],
                Prima = (decimal)row["Prima"]
            });
        }

        return View(seguros);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Seguro seguro)
    {
        if (!ModelState.IsValid)
        {
            return View(seguro);
        }

        string query = "INSERT INTO Seguros (Nombre, Codigo, SumaAsegurada, Prima) VALUES (@Nombre, @Codigo, @SumaAsegurada, @Prima)";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Nombre", seguro.Nombre),
            new SqlParameter("@Codigo", seguro.Codigo),
            new SqlParameter("@SumaAsegurada", seguro.SumaAsegurada),
            new SqlParameter("@Prima", seguro.Prima)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        string query = "SELECT * FROM Seguros WHERE SeguroId = @SeguroId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@SeguroId", id)
        };

        DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
        if (dt.Rows.Count == 0)
        {
            return NotFound();
        }

        DataRow row = dt.Rows[0];

        Seguro seguro = new Seguro
        {
            SeguroId = (int)row["SeguroId"],
            Nombre = row["Nombre"].ToString(),
            Codigo = row["Codigo"].ToString(),
            SumaAsegurada = (decimal)row["SumaAsegurada"],
            Prima = (decimal)row["Prima"]
        };

        return View(seguro);
    }

    [HttpPost]
    public IActionResult Edit(Seguro seguro)
    {
        if (!ModelState.IsValid)
        {
            return View(seguro);
        }

        string query = "UPDATE Seguros SET Nombre = @Nombre, Codigo = @Codigo, SumaAsegurada = @SumaAsegurada, Prima = @Prima WHERE SeguroId = @SeguroId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Nombre", seguro.Nombre),
            new SqlParameter("@Codigo", seguro.Codigo),
            new SqlParameter("@SumaAsegurada", seguro.SumaAsegurada),
            new SqlParameter("@Prima", seguro.Prima),
            new SqlParameter("@SeguroId", seguro.SeguroId)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        string query = "SELECT * FROM Seguros WHERE SeguroId = @SeguroId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@SeguroId", id)
        };

        DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
        if (dt.Rows.Count == 0)
        {
            return NotFound();
        }

        DataRow row = dt.Rows[0];
        Seguro seguro = new Seguro
        {
            SeguroId = (int)row["SeguroId"],
            Nombre = row["Nombre"].ToString(),
            Codigo = row["Codigo"].ToString(),
            SumaAsegurada = (decimal)row["SumaAsegurada"],
            Prima = (decimal)row["Prima"]
        };

        return View(seguro);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        string query = "DELETE FROM Seguros WHERE SeguroId = @SeguroId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@SeguroId", id)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return RedirectToAction(nameof(Index));
    }
}
