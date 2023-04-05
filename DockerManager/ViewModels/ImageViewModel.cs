using Docker.DotNet.Models;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace DockerManager.ViewModels
{
    public class ImageViewModel
    {

        /// <summary>
        /// Id：镜像ID
        //Repository：镜像仓库
        //Tag：镜像标签
        //Created：镜像创建时间
        //Size：镜像大小
        /// </summary>
        public string Id { get; set; }
        public string Repository { get; set; }
        public string RepoTags { get; set; }
        public string Created { get; set; }
        public string Size { get; set; }

        /// <summary>
        /// 指Docker镜像的虚拟大小，即包括镜像本身的大小以及镜像所引用的其他层的大小总和
        /// </summary>
        public string VirtualSize { get; set; }
        public ImageViewModel(ImagesListResponse image)
        {
            Id = image.ID;
            Repository = image.RepoTags.FirstOrDefault()?.Split(':')[0];
            RepoTags = image.RepoTags.FirstOrDefault()?.Split(':')[1];
            Created = image.Created.ToString("yyyy-MM-dd HH:mm:ss");
            //Created = GetCreatedString(image.Created);
            Size = GetSizeString(image.Size);
            VirtualSize = GetSizeString(image.VirtualSize);

        }

        private string GetCreatedString(long created)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(created).ToLocalTime();
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private string GetSizeString(long size)
        {
            var sizes = new string[] { "B", "KB", "MB", "GB", "TB" };
            var order = 0;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
    }
}
