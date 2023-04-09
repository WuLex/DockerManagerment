using Docker.DotNet;
using Docker.DotNet.Models;
using DockerManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace DockerManager.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDockerClient _dockerClient;

        public ManagerController(IDockerClient dockerClient, IConfiguration configuration)
        {
            _dockerClient = dockerClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var containers = new List<ContainerViewModel>();
            var images = new List<ImageViewModel>();

            #region 容器列表

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

            #region 镜像列表

            var responseListImages = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters { All = true });
            foreach (var image in responseListImages)
            {
                images.Add(new ImageViewModel(image));
            }

            #endregion 镜像

            ViewBag.Containers = containers;
            ViewBag.Images = images;

            //版本号
            var version = await _dockerClient.System.GetVersionAsync();
            ViewBag.Version = version.Version;

            return View();
        }

        /// <summary>
        /// Image Name: redis:latest
        /// Container Name: myrediscontainer
        /// Ports: 6379:6379
        /// Volumes: redis_data:/data
        ///
        /// 镜像名称：nginx
        //  容器名称：my-nginx-container
        /// 端口映射：80:80
        /// 其中端口映射的格式为：hostPort:containerPort，表示将主机上的80端口映射到容器的80端口。
        /// 在提交表单时，端口映射的输入框应该填写"80:80"，多个端口映射之间使用分号";"隔开，
        /// 例如"80:80;443:443"表示将主机上的80和443端口映射到容器的80和443端口。
        /// await CreateContainer("nginx", "my-nginx", "80:80", "/var/www/html:/usr/share/nginx/html");
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="containerName"></param>
        /// <param name="ports"></param>
        /// <param name="volumes"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateContainer(string imageName, string containerName, string ports, string volumes)
        {
            var portBindings = new Dictionary<string, IList<PortBinding>>();
            if (!string.IsNullOrEmpty(ports))
            {
                // Split the ports string into individual port mappings
                var portMappings = ports.Split(';');
                foreach (var portMapping in portMappings)
                {
                    // Split the individual port mapping into the host and container ports
                    var portMappingParts = portMapping.Split(':');
                    var hostPort = portMappingParts[0];
                    var containerPort = portMappingParts[1];

                    // Create a new port binding for this mapping
                    var portBinding = new PortBinding
                    {
                        HostPort = hostPort
                    };

                    // Add the port binding to the dictionary for the container port
                    if (!portBindings.ContainsKey(containerPort))
                    {
                        portBindings.Add(containerPort, new List<PortBinding>());
                    }
                    portBindings[containerPort].Add(portBinding);
                }
            }

            var hostConfig = new HostConfig();

            //卷挂载volumes不为空,则添加绑定
            if (!string.IsNullOrEmpty(volumes))
            {
                hostConfig.Binds = new List<string> { volumes };
            }

            var createParameters = new CreateContainerParameters
            {
                Image = imageName,
                Name = containerName,
                HostConfig = hostConfig,
                ExposedPorts = new Dictionary<string, EmptyStruct>()
            };

            if (portBindings.Count > 0)
            {
                // Add the exposed ports to the create parameters
                //为了使端口绑定生效，我们需要将端口添加到 ExposedPorts 字典中。
                //如果你不这样做，Docker 将无法将容器端口暴露给主机。
                foreach (var port in portBindings.Keys)
                {
                    createParameters.ExposedPorts.Add(port, new EmptyStruct());
                }

                // Add the port bindings to the host config
                hostConfig.PortBindings = portBindings;
            }

            var container = await _dockerClient.Containers.CreateContainerAsync(createParameters);

            await _dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 创建redis容器,测试用
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateRedisContainer()
        {
            //var dockerClient = new DockerClientConfiguration(new Uri( _configuration.GetValue<string>("DockerApiUrl"))).CreateClient();

            //var containerConfig = new Config
            //{
            //    Image = "redis:latest",
            //    ExposedPorts = new Dictionary<string, EmptyStruct>
            //                    {
            //                        { "6379/tcp", default }
            //                    }
            //};

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

            var containerCreateParameters = new CreateContainerParameters()
            {
                Image = "redis:latest",
                Name = "myrediscontainer",
                HostConfig = hostConfig,
                //NetworkingConfig = new NetworkingConfig()
            };

            //containerCreateParameters.HostConfig.PortBindings = new Dictionary<string, IList<PortBinding>>
            //                                                    {
            //                                                        {
            //                                                            "6379/tcp",
            //                                                            new List<PortBinding>
            //                                                            {
            //                                                                new PortBinding
            //                                                                {
            //                                                                    HostPort = "6379"
            //                                                                }
            //                                                            }
            //                                                        }
            //                                                    };

            //containerCreateParameters.NetworkingConfig.EndpointsConfig = new Dictionary<string, EndpointSettings>
            //                                                                {
            //                                                                    {
            //                                                                        "bridge",
            //                                                                        new EndpointSettings
            //                                                                        {
            //                                                                            IPAMConfig = new EndpointIPAMConfig
            //                                                                            {
            //                                                                                IPv4Address = "172.17.0.2"
            //                                                                            }
            //                                                                        }
            //                                                                    }
            //                                                                };

            var createdContainer = await _dockerClient.Containers.CreateContainerAsync(containerCreateParameters);
            var containerId = createdContainer.ID;
            //启动容器
            await _dockerClient.Containers.StartContainerAsync(containerId, null);

            //return containerId;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 删除容器
        /// </summary>
        /// <param name="containerId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteContainer(string containerId)
        {
            await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true });

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 创建公共仓库的镜像
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 删除镜像
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteImage(string imageId)
        {
            await _dockerClient.Images.DeleteImageAsync(imageId, new ImageDeleteParameters { Force = true });

            return RedirectToAction("Index");
        }

        #region 废弃
        //[HttpPost]
        //public async Task<IActionResult> CreateContainer(string imageName, string containerName)
        //{
        //    var container = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        //    {
        //        Image = imageName,
        //        Name = containerName
        //    });

        //    return RedirectToAction("Index");
        //} 
        #endregion
        #region 废弃
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
        #endregion

    }
}