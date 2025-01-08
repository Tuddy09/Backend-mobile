namespace Backend_mobile.Models
{
    public class CarMaintenanceRecord
    {
        public int Id { get; set; }
        public string CarModel { get; set; }
        public string ServiceType { get; set; }
        public string ServiceDate { get; set; }
        public string ServiceNotes { get; set; }
    }
}
