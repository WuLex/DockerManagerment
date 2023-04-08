using Docker.DotNet;
using Docker.DotNet.Models;
using DockerManager.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DockerManager.Utils
{
    public static class DockerHelper
    {

        public static readonly DockerClient myClient = new DockerClientConfiguration(new Uri("http://192.168.1.103:2375/")).CreateClient();


        #region 方式一
        public static async Task<ContainerStatsResponse> GetContainerStats(string containerId)
        {
            using (var client = new DockerClientConfiguration(new Uri("http://192.168.1.103:2375/")).CreateClient())
            {
                var parameters = new ContainerStatsParameters { Stream = false };
                var progress = new ContainerStatsProgress();
                await client.Containers.GetContainerStatsAsync(containerId, parameters, progress);
                return await progress.Task;
            }
        }
        #endregion

        #region
        //public static async Task<ContainerStatsResponse> GetContainerStats(string containerId)
        //{
        //    using (var client = new DockerClientConfiguration().CreateClient())
        //    {
        //        var cancellationTokenSource = new CancellationTokenSource();
        //        var parameters = new ContainerStatsParameters() { Stream = false };
        //        var progress = new Progress<ContainerStatsResponse>(response => {
        //            // 在这里处理响应的回调
        //            // 更新异步方法中的状态
        //        });

        //        var progress2 = new Progress<ContainerStatsResponse>();
        //        progress2.ProgressChanged += async (obj, message) =>
        //        {
        //        };
        //        await client.Containers.GetContainerStatsAsync(
        //            containerId,
        //            new ContainerStatsParameters
        //            {
        //                //TODO:
        //            },
        //            progress,
        //            cancellationTokenSource.Token
        //        );
        //        return  null;
        //    }
        //}
        #endregion

        public static async Task<SystemInfoResponse> GetSystemInfo()
        {
          
             return await myClient.System.GetSystemInfoAsync();
        }

        public static async Task<VersionResponse> GetVersion()
        {
          
             return await myClient.System.GetVersionAsync();
        }


        /// <summary>
        /// 代码中使用的是基于 Windows 操作系统的 Docker 引擎的本地管道（npipe）。
        /// 如果您的操作系统是 Linux 或 macOS，则应将管道 URI 更改为 unix:///var/run/docker.sock。
        /// </summary>
        /// <returns></returns>
        public static async Task<List<NetworkResponse>> GetNetworks()
        {
            //var config = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine"));
            //var config = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")); 
            var config = new DockerClientConfiguration(new Uri("http://192.168.1.103:2375/")); 
            using var client = config.CreateClient();
            var networks = (await client.Networks.ListNetworksAsync(new NetworksListParameters())).ToList();
            return  networks;
        }

        public class ContainerStatsProgress : IProgress<ContainerStatsResponse>
        {
            private TaskCompletionSource<ContainerStatsResponse> _tcs;

            public Task<ContainerStatsResponse> Task => _tcs.Task;

            public ContainerStatsProgress()
            {
                _tcs = new TaskCompletionSource<ContainerStatsResponse>();
            }

            public void Report(ContainerStatsResponse stats)
            {
                // 在这里实现进度更新的逻辑，例如输出容器统计信息
                Console.WriteLine($"Container CPU usage: {stats.CPUStats.CPUUsage.PercpuUsage?[0]}");

                // 完成任务，将统计信息作为结果
                _tcs.SetResult(stats);
            }
        }
    }
}
