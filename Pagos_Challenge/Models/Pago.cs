using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pagos_Challenge.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public string pago { get; set; }
        public DateTime fecha { get; set; }
        public int importe { get; set; }
    }
}