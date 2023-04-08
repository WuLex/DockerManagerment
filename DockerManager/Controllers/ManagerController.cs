using Docker.DotNet;
using Docker.DotNet.Models;
using DockerManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DockerManager.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IDockerClient _dockerClient;

        public ManagerController(IDockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }

        public async Task<IActionResult> Index()
        {
            var containers = new List<ContainerViewModel>();
            var images = new List<ImageViewModel>();

            #region 容器

            var response = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true });
            foreach (var container in response)
            {
                containers.Add(new ContainerViewModel
                {
                    Id = container.ID,
                    Name = container.Names[0],
                    State = container.State,
                    Status = container.Status,
                    Created = container.Created,
                    Image = container.Image
                });
            }

            #endregion 容器

            #region 镜像

            var responseListImages = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters { All = true });
            foreach (var image in responseListImages)
            {
                images.Add(new ImageViewModel(image));
            }

            #endregion 镜像

            ViewBag.Containers = containers;
            ViewBag.Images = images;

            var version = await _dockerClient.System.GetVersionAsync();
            ViewBag.Version = version.Version;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(string imageName, string containerName)
        {
            var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = imageName,
                Name = containerName
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateRedisContainer()   //public async Task<string> CreateRedisContainer()
        {
            var dockerClient = new DockerClientConfiguration().CreateClient();

            var containerConfig = new Config
            {
                Image = "redis:latest",
                ExposedPorts = new Dictionary<string, EmptyStruct>
                                {
                                    { "6379/tcp", default }
                                }
            };

            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                                {
                                    {
                                        "6379/tcp", new List<PortBinding>
                                        {
                                            new PortBinding
                                            {
                                                HostPort = "6379"
                                            }
                                        }
                                    }
                                }
            };

            var containerCreateParameters = new CreateContainerParameters(containerConfig)
            {
                Name = "my-redis-container",
                //Config = containerConfig,
                HostConfig = hostConfig
            };

            var createdContainer = await dockerClient.Containers.CreateContainerAsync(containerCreateParameters);
            var containerId = createdContainer.ID;
            //启动容器
            await dockerClient.Containers.StartContainerAsync(containerId, null);

            //return containerId;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContainer(string containerId)
        {
            await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateImage(string imageName)
        {
            //var tarPath = System.IO.Directory.GetCurrentDirectory() + $"/Dockerfiles/{imageName}Dockerfile";
            //using (var stream = new FileStream(tarPath, FileMode.Open))
            //{
            //    //var task = client.Images.BuildImageFromDockerfileAsync(stream, new ImageBuildParameters() { Dockerfile = serviceDockerfile[service], Tags = new string[] { serviceImage[service] } });
            //    //task.Wait();
            //    await _dockerClient.Images.BuildImageFromDockerfileAsync(stream, new ImageBuildParameters
            //    {
            //        Dockerfile = "Dockerfile",
            //        Tags = new List<string> { imageName }
            //    });
            //}

            #region 设置 AuthConfig 参数

            //但是，如果您使用私有注册表，则需要使用适当的身份验证凭据设置 AuthConfig 参数。 否则，您可能会在尝试将镜像拉取或推送到注册表时遇到身份验证错误。
            //总之，如果您使用的是公共 Docker Hub 注册表，则无需设置 AuthConfig 参数。 但是，如果您使用私有注册表，则有必要使用 AuthConfig 参数提供身份验证凭据。
            //var authConfig = new AuthConfig
            //{
            //    Username = "your_username",
            //    Password = "your_password"
            //};

            //await dockerClient.Images.CreateImageAsync(imageCreateParameters, authConfig, new Progress<JSONMessage>());

            #endregion 设置 AuthConfig 参数

            var imageCreateParameters = new ImagesCreateParameters
            {
                FromImage = imageName,
                Tag = "latest"
            };
            try
            {
                await _dockerClient.Images.CreateImageAsync(imageCreateParameters, null, new Progress<JSONMessage>());
                return RedirectToAction("Index");
            }
            catch (DockerApiException ex)
            {
                Console.WriteLine($"镜像创建失败: {ex.Message}");
                return BadRequest("镜像创建失败");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateImage(string imageName)
        //{
        //    var tarPath = System.IO.Directory.GetCurrentDirectory() + $"/Dockerfiles/{imageName}Dockerfile";
        //    using (var stream = new FileStream(tarPath, FileMode.Open))
        //    {
        //        var authConfig = new AuthConfig
        //        {
        //            Username = "your-registry-username",
        //            Password = "your-registry-password"
        //        };
        //        var imageBuildParameters = new ImageBuildParameters
        //        {
        //            Dockerfile = "Dockerfile",
        //            Tags = new List<string> { imageName }
        //        };
        //        var headers = new Dictionary<string, string>
        //{
        //    { "Cache-Control", "no-cache" }
        //};
        //        var progress = new Progress<JSONMessage>();
        //        var cancellationToken = CancellationToken.None;

        //        await _dockerClient.Images.BuildImageFromDockerfileAsync(stream, imageBuildParameters, authConfig, headers, progress, cancellationToken);
        //    }

        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteImage(string imageId)
        {
            await _dockerClient.Images.DeleteImageAsync(imageId, new ImageDeleteParameters { Force = true });

            return RedirectToAction("Index");
        }
    }
}