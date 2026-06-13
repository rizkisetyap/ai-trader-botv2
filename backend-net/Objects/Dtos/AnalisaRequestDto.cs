namespace backend_net.Objects.Dtos
{
    public class AnalisaRequestDto
    
    {
       public string Ticker { get; set; }
        public string PrediksiTeknikal { get; set; }
        public decimal ProbabilitasKenaikan { get; set; }
        public string Sentimen {get; set; }
        public DateTime Tanggal { get; set; }
    }
}