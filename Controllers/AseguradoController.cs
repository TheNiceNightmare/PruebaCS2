using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SegurosChubbi.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

public class AseguradoController : Controller
{
    private readonly SqlDbHelper _dbHelper;

    public AseguradoController(IConfiguration configuration)
    {
        _dbHelper = new SqlDbHelper(configuration);
    }

    public IActionResult Index()
    {
        string query = "SELECT * FROM Asegurados";
        DataTable dt = _dbHelper.ExecuteQuery(query);
        List<Asegurado> asegurados = new List<Asegurado>();

        foreach (DataRow row in dt.Rows)
        {
            asegurados.Add(new Asegurado
            {
                AseguradoId = (int)row["AseguradoId"],
                Cedula = row["Cedula"].ToString(),
                Nombre = row["Nombre"].ToString(),
                Telefono = row["Telefono"].ToString(),
                Edad = (int)row["Edad"]
            });
        }

        return View(asegurados);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Asegurado asegurado)
    {
        string query = "INSERT INTO Asegurados (Cedula, Nombre, Telefono, Edad) VALUES (@Cedula, @Nombre, @Telefono, @Edad)";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Cedula", asegurado.Cedula),
            new SqlParameter("@Nombre", asegurado.Nombre),
            new SqlParameter("@Telefono", asegurado.Telefono),
            new SqlParameter("@Edad", asegurado.Edad)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        string query = "SELECT * FROM Asegurados WHERE AseguradoId = @AseguradoId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@AseguradoId", id)
        };

        DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
        DataRow row = dt.Rows[0];

        Asegurado asegurado = new Asegurado
        {
            AseguradoId = (int)row["AseguradoId"],
            Cedula = row["Cedula"].ToString(),
            Nombre = row["Nombre"].ToString(),
            Telefono = row["Telefono"].ToString(),
            Edad = (int)row["Edad"]
        };

        return View(asegurado);
    }

    [HttpPost]
    public IActionResult Edit(Asegurado asegurado)
    {
        string query = "UPDATE Asegurados SET Cedula = @Cedula, Nombre = @Nombre, Telefono = @Telefono, Edad = @Edad WHERE AseguradoId = @AseguradoId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Cedula", asegurado.Cedula),
            new SqlParameter("@Nombre", asegurado.Nombre),
            new SqlParameter("@Telefono", asegurado.Telefono),
            new SqlParameter("@Edad", asegurado.Edad),
            new SqlParameter("@AseguradoId", asegurado.AseguradoId)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        string query = "DELETE FROM Asegurados WHERE AseguradoId = @AseguradoId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@AseguradoId", id)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return RedirectToAction(nameof(Index));
    }
}
