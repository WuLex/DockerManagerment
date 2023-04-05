using Docker.DotNet.Models;
using Docker.DotNet;
using DockerManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DockerManager.ViewModels;

namespace DockerManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDockerClient _dockerClient;

        public HomeController(ILogger<HomeController> logger, IDockerClient dockerClient)
        {
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
            var containerViewModels = containers.Select(c => new ContainerViewModel
            {
                Id = c.ID,
                Name = c.Names.FirstOrDefault(),
                State = c.State,
                Status = c.Status,
                Created = c.Created,
                Image = c.Image
            }).ToList();

            return View(containerViewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<string> GetDockerVersionAsync()
        {
            //var client = new DockerClientConfiguration().CreateClient();
            var version = await _dockerClient.System.GetVersionAsync();
            return version.Version;
        }

        public async Task<string> CreateContainerAsync(string imageName, string containerName)
        {
            //var client = new DockerClientConfiguration().CreateClient();
            var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                Name = containerName
            });
            return container.ID;
        }
    }
}