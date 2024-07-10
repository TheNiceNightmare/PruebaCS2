using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SegurosChubbi.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

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
        ViewBag.Seguros = GetSegurosSelectList();
        return View();
    }

    [HttpPost]
    public IActionResult Create(Asegurado asegurado, int[] segurosSeleccionados)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Seguros = GetSegurosSelectList();
            return View(asegurado);
        }

        string query = "INSERT INTO Asegurados (Cedula, Nombre, Telefono, Edad) VALUES (@Cedula, @Nombre, @Telefono, @Edad)";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@Cedula", asegurado.Cedula),
            new SqlParameter("@Nombre", asegurado.Nombre),
            new SqlParameter("@Telefono", asegurado.Telefono),
            new SqlParameter("@Edad", asegurado.Edad)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        int aseguradoId = GetLastInsertedId();

        foreach (var seguroId in segurosSeleccionados)
        {
            string assignSeguroQuery = "INSERT INTO AseguradoSeguro (AseguradoId, SeguroId) VALUES (@AseguradoId, @SeguroId)";
            SqlParameter[] assignParameters = new SqlParameter[]
            {
                new SqlParameter("@AseguradoId", aseguradoId),
                new SqlParameter("@SeguroId", seguroId)
            };

            _dbHelper.ExecuteNonQuery(assignSeguroQuery, assignParameters);
        }

        return RedirectToAction(nameof(Index));
    }

    private SelectList GetSegurosSelectList()
    {
        string query = "SELECT SeguroId, Codigo FROM Seguros";
        DataTable dt = _dbHelper.ExecuteQuery(query);
        List<SelectListItem> seguros = new List<SelectListItem>();

        foreach (DataRow row in dt.Rows)
        {
            seguros.Add(new SelectListItem
            {
                Value = row["SeguroId"].ToString(),
                Text = row["Codigo"].ToString()
            });
        }

        return new SelectList(seguros, "Value", "Text");
    }

    private int GetLastInsertedId()
    {
        string query = "SELECT TOP 1 AseguradoId FROM Asegurados ORDER BY AseguradoId DESC";
        object result = _dbHelper.ExecuteScalar(query);

        if (result == null || result == DBNull.Value)
        {
            throw new InvalidOperationException("No se pudo obtener el último ID insertado.");
        }

        return Convert.ToInt32(result);
    }

    public IActionResult Edit(int id)
    {
        string query = "SELECT * FROM Asegurados WHERE AseguradoId = @AseguradoId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@AseguradoId", id)
        };

        DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
        if (dt.Rows.Count == 0)
        {
            return NotFound();
        }

        DataRow row = dt.Rows[0];

        Asegurado asegurado = new Asegurado
        {
            AseguradoId = (int)row["AseguradoId"],
            Cedula = row["Cedula"].ToString(),
            Nombre = row["Nombre"].ToString(),
            Telefono = row["Telefono"].ToString(),
            Edad = (int)row["Edad"]
        };

        ViewBag.Seguros = GetSegurosSelectList();
        return View(asegurado);
    }

    [HttpPost]
    public IActionResult Edit(Asegurado asegurado, int[] segurosSeleccionados)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Seguros = GetSegurosSelectList();
            return View(asegurado);
        }

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

        // Eliminar relaciones existentes
        string deleteQuery = "DELETE FROM AseguradoSeguro WHERE AseguradoId = @AseguradoId";
        SqlParameter[] deleteParameters = new SqlParameter[]
        {
            new SqlParameter("@AseguradoId", asegurado.AseguradoId)
        };
        _dbHelper.ExecuteNonQuery(deleteQuery, deleteParameters);

        // Asignar nuevos seguros
        foreach (var seguroId in segurosSeleccionados)
        {
            string assignSeguroQuery = "INSERT INTO AseguradoSeguro (AseguradoId, SeguroId) VALUES (@AseguradoId, @SeguroId)";
            SqlParameter[] assignParameters = new SqlParameter[]
            {
                new SqlParameter("@AseguradoId", asegurado.AseguradoId),
                new SqlParameter("@SeguroId", seguroId)
            };

            _dbHelper.ExecuteNonQuery(assignSeguroQuery, assignParameters);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        string query = "SELECT * FROM Asegurados WHERE AseguradoId = @AseguradoId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@AseguradoId", id)
        };

        DataTable dt = _dbHelper.ExecuteQuery(query, parameters);
        if (dt.Rows.Count == 0)
        {
            return NotFound();
        }

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

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        string query = "DELETE FROM Asegurados WHERE AseguradoId = @AseguradoId";
        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@AseguradoId", id)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        string deleteRelationQuery = "DELETE FROM AseguradoSeguro WHERE AseguradoId = @AseguradoId";
        SqlParameter[] deleteRelationParameters = new SqlParameter[]
        {
            new SqlParameter("@AseguradoId", id)
        };

        _dbHelper.ExecuteNonQuery(deleteRelationQuery, deleteRelationParameters);

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Upload(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var fields = line.Split(',');
                    var asegurado = new Asegurado
                    {
                        Cedula = fields[0],
                        Nombre = fields[1],
                        Telefono = fields[2],
                        Edad = int.Parse(fields[3])
                    };

                    // Inserción del asegurado
                    string query = "INSERT INTO Asegurados (Cedula, Nombre, Telefono, Edad) VALUES (@Cedula, @Nombre, @Telefono, @Edad)";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Cedula", asegurado.Cedula),
                        new SqlParameter("@Nombre", asegurado.Nombre),
                        new SqlParameter("@Telefono", asegurado.Telefono),
                        new SqlParameter("@Edad", asegurado.Edad)
                    };

                    _dbHelper.ExecuteNonQuery(query, parameters);

                    try
                    {
                        int aseguradoId = GetLastInsertedId();

                        // Asignación automática de productos según la edad
                        string asignacionQuery = "";
                        if (asegurado.Edad < 20)
                        {
                            asignacionQuery = "INSERT INTO AseguradoSeguro (AseguradoId, SeguroId) VALUES (@AseguradoId, 1)"; // Producto 1
                        }
                        else if (asegurado.Edad >= 20 && asegurado.Edad <= 30)
                        {
                            asignacionQuery = "INSERT INTO AseguradoSeguro (AseguradoId, SeguroId) VALUES (@AseguradoId, 2)"; // Producto 2
                        }
                        // Agregar más condiciones según sea necesario

                        SqlParameter[] asignacionParameters = new SqlParameter[]
                        {
                            new SqlParameter("@AseguradoId", aseguradoId)
                        };

                        _dbHelper.ExecuteNonQuery(asignacionQuery, asignacionParameters);
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }
}
