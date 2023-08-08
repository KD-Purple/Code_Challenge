using Pagos_Challenge.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Pagos_Challenge.Controllers
{
    public class PagoController : Controller
    {
        //Llamando la conexion. Tomada desde Web.config
        private static string link = ConfigurationManager.ConnectionStrings["cadena"].ToString();
        private static List<Pago> pagolist = new List<Pago>();


        // GET: Pago
        public ActionResult Index()
        {
            pagolist = new List<Pago>();

            //Declarando la conexion.
            using (SqlConnection conexion = new SqlConnection(link))
            {
                //Declarando el query
                SqlCommand cmd = new SqlCommand("SELECT * FROM pago", conexion);
                cmd.CommandType = CommandType.Text;
                //Inicia la conexion
                conexion.Open();

                //Obtiene los datos usando el query
                using (SqlDataReader obtener = cmd.ExecuteReader())
                {
                    while (obtener.Read())
                    {
                        //Usa el modelo para recibir la informacion
                        Pago receptor = new Pago();
                        receptor.IdPago = int.Parse(obtener["IdPago"].ToString());
                        receptor.pago = obtener["pago"].ToString();
                        receptor.fecha = DateTime.Parse(obtener["fecha"].ToString());
                        receptor.importe = int.Parse(obtener["importe"].ToString());

                        //Llena la lista declarada para retornarla a la vista.
                        pagolist.Add(receptor);
                    }
                }
            }

                return View(pagolist);
        }

        public ActionResult RegistrarPago()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarPago(Pago p)
        {
            // Validar que el campo "pago" no se repita en la base de datos
            if (EsPagoUnico(p.pago))
            {
                // Si el pago es único, insertar los datos en la base de datos
                InsertarPagoEnBD(p);
                return RedirectToAction("Index", "Pago");
            }
            else
            {
                // Si el pago ya existe, mostrar un mensaje de error en la vista
                TempData["MensajeError"] = "El pago ya existe en la base de datos.";
                return View(p);
            }

            
        }

        private bool EsPagoUnico(string pago)
        {
            using (SqlConnection conexion = new SqlConnection(link))
            {
                // Consulta SQL para verificar si el pago ya existe en la base de datos
                string sqlQuery = "SELECT COUNT(*) FROM Pago WHERE pago = @pago";

                // Crear y abrir la conexión a la base de datos
                conexion.Open();

                // Crear un objeto SqlCommand con la consulta SQL y los parámetros
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conexion))
                {
                    // Agregar el parámetro a la consulta SQL
                    cmd.Parameters.AddWithValue("pago", pago);

                    // Ejecutar la consulta y obtener el número de filas que coinciden con el pago
                    int count = (int)cmd.ExecuteScalar();

                    // Si count es igual a 0, el pago es único; de lo contrario, ya existe en la base de datos
                    return count == 0;
                }
            }
        }

        private void InsertarPagoEnBD(Pago p)
        {
            using (SqlConnection conexion = new SqlConnection(link))
            {
                SqlCommand cmd = new SqlCommand("p_Registrar", conexion);
                cmd.Parameters.AddWithValue("pago", p.pago);
                cmd.Parameters.AddWithValue("fecha", p.fecha);
                cmd.Parameters.AddWithValue("importe", p.importe);

                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                cmd.ExecuteNonQuery();

            }
        }

            public ActionResult EditarPago(int? idPago)
        {
            if(idPago == null)
                return RedirectToAction("Index", "Pago");

            Pago pago = pagolist.Where(c => c.IdPago == idPago).FirstOrDefault();

            return View(pago);
        }

        [HttpPost]
        public ActionResult EditarPago(Pago p)
        {
            using (SqlConnection conexion = new SqlConnection(link))
            {
                SqlCommand cmd = new SqlCommand("p_Editar", conexion);
                cmd.Parameters.AddWithValue("IdPago", p.IdPago);
                cmd.Parameters.AddWithValue("pago", p.pago);
                cmd.Parameters.AddWithValue("fecha", p.fecha);
                cmd.Parameters.AddWithValue("importe", p.importe);

                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                cmd.ExecuteNonQuery();

            }

            return RedirectToAction("Index", "Pago");
        }

        public ActionResult BorrarPago(int? idPago)
        {
            if (idPago == null)
                return RedirectToAction("Index", "Pago");

            Pago pago = pagolist.Where(c => c.IdPago == idPago).FirstOrDefault();

            return View(pago);
        }

        [HttpPost]
        public ActionResult BorrarPago(string idPago)
        {
            using (SqlConnection conexion = new SqlConnection(link))
            {
                SqlCommand cmd = new SqlCommand("p_Borrar", conexion);
                cmd.Parameters.AddWithValue("IdPago", idPago);

                cmd.CommandType = CommandType.StoredProcedure;
                conexion.Open();
                cmd.ExecuteNonQuery();

            }

            return RedirectToAction("Index", "Pago");
        }

        //public ActionResult MultipleBorrar()
        //{
        //    return View();
        //}

        // Método para eliminar múltiples registros
        [HttpPost]
        public ActionResult MultipleBorrar(List<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                // Realiza la eliminación de los registros en la base de datos
                string deleteQuery = "DELETE FROM pago WHERE IdPago IN (@Ids)";
                using (SqlConnection conexion = new SqlConnection(link))
                {
                    SqlCommand cmd = new SqlCommand(deleteQuery, conexion);
                    cmd.Parameters.AddWithValue("@Ids", string.Join(",", ids));

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    conexion.Close();
                }
            }

            return RedirectToAction("Index", "Pago");
        }
    }
}