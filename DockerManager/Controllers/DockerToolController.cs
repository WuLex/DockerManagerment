using Docker.DotNet;
using Docker.DotNet.Models;
using DockerManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DockerManager.Controllers
{
    public class DockerToolController : Controller
    {
        private readonly IDockerClient _dockerClient;

        public DockerToolController(IDockerClient dockerClient)
        {
            _dockerClient = dockerClient;
            //_dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"))
            //    .CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            // Get docker version info
            var versionResponse = await _dockerClient.System.GetVersionAsync();
            ViewBag.Version = $"{versionResponse.Version}";

            // Get all containers
            var containerListResponse = await _dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    All = true
                });
            var containers = new List<ContainerViewModel>();
            foreach (var container in containerListResponse)
            {
                var containerDetails = await _dockerClient.Containers.InspectContainerAsync(container.ID);
                containers.Add(new ContainerViewModel()
                {
                    Id = container.ID,
                    Name = container.Names.FirstOrDefault(),
                    State = container.State,
                    Status = containerDetails.State?.Status,
                    Created = container.Created,
                    Image = containerDetails.Image
                });
            }
            ViewBag.Containers = containers;

            // Get all images
            var imagesResponse = await _dockerClient.Images.ListImagesAsync(
                new ImagesListParameters()
                {
                    All = true
                });
            ViewBag.Images = imagesResponse;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(string imageName, string containerName)
        {
            var createdContainer = await _dockerClient.Containers.CreateContainerAsync(
                new CreateContainerParameters()
                {
                    Image = imageName,
                    Name = containerName
                });
            await _dockerClient.Containers.StartContainerAsync(createdContainer.ID, new ContainerStartParameters());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContainer(string containerId)
        {
            await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateImage(string imageName)
        {
            await _dockerClient.Images.CreateImageAsync(
               new ImagesCreateParameters()
               {
                   FromImage = imageName
               },
               new AuthConfig(),
               new Progress<JSONMessage>());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(string imageId)
        {
            await _dockerClient.Images.DeleteImageAsync(imageId, new ImageDeleteParameters());
            return RedirectToAction(nameof(Index));
        }
    }
}