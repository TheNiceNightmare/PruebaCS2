namespace SegurosChubbi.Models
{
    public class Seguro
    {
        public int SeguroId { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public decimal SumaAsegurada { get; set; }
        public decimal Prima { get; set; }
    }
}
