namespace DockerManager.Models
{
    public class DockerContainer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Command { get; set; }
        public string Created { get; set; }
        public string Status { get; set; }
    }
}
