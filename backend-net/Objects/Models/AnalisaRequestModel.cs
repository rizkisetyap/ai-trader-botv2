namespace backend_net.Objects.Models
{
    public class AnalisaRequestModel
    {
        public Guid Id { get; set; }
        public string Ticker { get; set; }
        public string PrediksiTeknikal { get; set; }
        public decimal ProbabilitasKenaikan { get; set; }
        public string Sentimen {get; set; }
        public DateTime Tanggal { get; set; }
    }
}