using DockerManager.Utils;
using DockerManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DockerManager.Controllers
{
    public class DockerController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var containerid = "19408eb276e6";
            var containerStats = await DockerHelper.GetContainerStats(containerid);
            var systemInfo = await DockerHelper.GetSystemInfo();
            var version = await DockerHelper.GetVersion();

            var model = new DockerViewModel
            {
                ContainerStats = containerStats,
                SystemInfo = systemInfo,
                Version = version
            };

            return View(model);
        }

        public async Task<IActionResult> NetworkInfo()
        {
            var networks = await DockerHelper.GetNetworks();

            var model = new DockerNetworkViewModel
            {
                Networks = networks
            };

            return View(model);
        }
    }
}
