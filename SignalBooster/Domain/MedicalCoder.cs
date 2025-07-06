namespace Synapse.SignalBoosterExample.Models
{
    public class MedicalOrder
    {
        public string Device { get; set; }
        public string MaskType { get; set; }
        public string[] AddOns { get; set; }
        public string Qualifier { get; set; }
        public string OrderingProvider { get; set; }
        public string Liters { get; set; }
        public string Usage { get; set; }
    }
}