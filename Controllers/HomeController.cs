using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SegurosChubbi.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

public class HomeController : Controller
{
    private readonly SqlDbHelper _dbHelper;

    public HomeController(IConfiguration configuration)
    {
        _dbHelper = new SqlDbHelper(configuration);
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Consultar(string cedula, string codigoSeguro)
    {
        List<dynamic> resultados = new List<dynamic>();

        if (!string.IsNullOrEmpty(cedula))
        {
            string query = @"
                SELECT s.Nombre, s.Codigo, s.SumaAsegurada, s.Prima
                FROM Asegurados a
                INNER JOIN AseguradoSeguro asg ON a.AseguradoId = asg.AseguradoId
                INNER JOIN Seguros s ON asg.SeguroId = s.SeguroId
                WHERE a.Cedula = @Cedula";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Cedula", cedula)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in dt.Rows)
            {
                resultados.Add(new Seguro
                {
                    Nombre = row["Nombre"].ToString(),
                    Codigo = row["Codigo"].ToString(),
                    SumaAsegurada = (decimal)row["SumaAsegurada"],
                    Prima = (decimal)row["Prima"]
                });
            }
        }
        else if (!string.IsNullOrEmpty(codigoSeguro))
        {
            string query = @"
                SELECT a.Cedula, a.Nombre, a.Telefono, a.Edad
                FROM Seguros s
                INNER JOIN AseguradoSeguro asg ON s.SeguroId = asg.SeguroId
                INNER JOIN Asegurados a ON asg.AseguradoId = a.AseguradoId
                WHERE s.Codigo = @Codigo";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Codigo", codigoSeguro)
            };

            DataTable dt = _dbHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in dt.Rows)
            {
                resultados.Add(new Asegurado
                {
                    Cedula = row["Cedula"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Telefono = row["Telefono"].ToString(),
                    Edad = (int)row["Edad"]
                });
            }
        }

        return View("ResultadoConsulta", resultados);
    }
}
