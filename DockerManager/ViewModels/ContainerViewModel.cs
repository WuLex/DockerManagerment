namespace DockerManager.ViewModels
{
    public class ContainerViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }
        public string Image { get; set; }
    }
}
