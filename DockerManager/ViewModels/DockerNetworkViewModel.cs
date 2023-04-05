using Docker.DotNet.Models;
using Microsoft.AspNetCore.HttpOverrides;

namespace DockerManager.ViewModels
{
    /// <summary>
    /// 在Docker.DotNet SDK中，DockerClient类提供了许多操作Docker网络的方法，如创建网络、列出网络、删除网络、连接容器到网络等。这些方法返回的响应数据对应于以下类：
    //NetworkResponse    类表示与docker network ls命令相对应的网络列表。
    //NetworksCreateResponse 类表示与docker network create命令相对应的创建网络的响应。
    // 类表示与docker network inspect命令相对应的网络详细信息的响应。
    // 类表示与docker network connect命令相对应的将容器连接到网络的响应。
    // 类表示与docker network disconnect命令相对应的将容器从网络中断开连接的响应。
    /// </summary>
    public class DockerNetworkViewModel
    {
        public List<NetworkResponse> Networks { get; set; }
        //public List<NetworksCreateResponse> NetworksCreate { get; set; }
    }
}
