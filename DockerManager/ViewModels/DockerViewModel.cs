using Docker.DotNet.Models;

namespace DockerManager.ViewModels
{
    //public class DockerViewModel
    //{
    //    public string DockerVersion { get; set; }
    //    public string Os { get; set; }
    //    public string KernelVersion { get; set; }
    //    public string Architecture { get; set; }
    //    public List<DockerContainer> Containers { get; set; }
    //    public List<DockerImage> Images { get; set; }
    //    public List<DockerNetwork> Networks { get; set; }
    //}

    public class DockerViewModel
    {

        //docker stats 返回关于正在运行的Docker容器资源使用情况的实时统计信息，包括CPU使用率、内存使用量、网络流量等。该命令对应于Docker API的 ContainerStats 类。
        public ContainerStatsResponse ContainerStats { get; set; }
        //docker info 返回有关Docker守护程序的配置和状态信息，例如Docker守护程序版本、存储驱动程序、容器数量等。该命令对应于Docker API的 SystemInfo 类。
        public SystemInfoResponse SystemInfo { get; set; }
        //docker version 返回有关Docker客户端和服务端的版本信息，包括版本号、构建信息和API版本。该命令不对应特定的Docker API类，但是版本信息可以通过Docker API的 SystemInfo 类的相应字段获得。
        public VersionResponse Version { get; set; }
    }
}
