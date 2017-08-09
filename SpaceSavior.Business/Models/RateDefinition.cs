namespace SpaceSavior.Business.Models
{
    /// <summary>
    /// DTO for JSON deserialization of a single rate definition.
    /// </summary>
    public class RateDefinition
    {
        public string Days { get; set; }
        public string Times { get; set; }
        public int Price { get; set; }
    }
}
