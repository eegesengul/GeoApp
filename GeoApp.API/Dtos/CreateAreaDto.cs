namespace GeoApp.API.Dtos
{
    public class CreateAreaDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string WKTGeometry { get; set; } = string.Empty;

    }
}
