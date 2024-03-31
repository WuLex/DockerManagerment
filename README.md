# DockerManagerment

## 结果如图：

![img](https://raw.githubusercontent.com/WuLex/UsefulPicture/main/dockerwebmanager/result2.jpg)

![img](https://raw.githubusercontent.com/WuLex/UsefulPicture/main/dockerwebmanager/result3.png)


# Linux中配置和命令
在`/usr/lib/systemd/system/docker.service`，配置远程访问
```shell
yum -y install vim*
vim /usr/lib/systemd/system/docker.service
```
修改成:
```shell
ExecStart=/usr/bin/dockerd -H fd:// --containerd=/run/containerd/containerd.sock -H tcp://0.0.0.0:2375 -H unix:///var/run/docker.sock
```
## 重启 Docker：
```shell
systemctl daemon-reload && systemctl restart docker 
```
## 查看是否成功监听
```shell
$ netstat -lnpt|grep docker
```
## 开放防火墙
```shell
$ firewall-cmd --add-port=2375/tcp --permanent
$ firewall-cmd --reload
```
## 或者直接关闭防火墙
 
临时关闭命令： `systemctl stop firewalld`

永久关闭命令： `systemctl disable firewalld`

## 查看IP
```shell
ip addr
```
## 解决docker容器里网络请求慢的问题
如果请求的是自己内网的api, 可以直接修改`/etc/hosts`文件，如果是外网的请求可以通过更改`/etc/resolv.conf`里的`nameserver`实现。

解决方法： 
```shell
vim /etc/resolv.conf
```
添加 `nameserver 114.114.114.114`

使用`:wq` 保存文件

该文件是DNS域名解析的配置文件，它的格式很简单，每行以一个关键字开头，后接配置参数。
`resolv.conf`的关键字主要有四个，分别是：
```shell
`nameserver`   #定义DNS服务器的IP地址
`domain`       #定义本地域名
`search`       #定义域名的搜索列表
`sortlist`     #对返回的域名进行排序
```

# Docker常用命令

以下是一些常用的Docker命令，以Markdown表格的形式输出：

| 命令 | 描述 |
| ---- | ---- |
| `docker build [OPTIONS] PATH \| URL \| -` | 根据 Dockerfile 创建一个镜像 |
| `docker run [OPTIONS] IMAGE [COMMAND] [ARG...]` | 从镜像创建一个新的容器并运行一个命令 |
| `docker ps [OPTIONS]` | 列出所有正在运行的容器 |
| `docker ps -a` | 列出所有容器，包括停止的容器 |
| `docker images [OPTIONS]` | 列出所有本地的镜像 |
| `docker pull [OPTIONS] NAME[:TAG\|@DIGEST]` | 从镜像仓库中拉取或更新指定镜像 |
| `docker push [OPTIONS] NAME[:TAG]` | 将镜像推送到镜像仓库 |
| `docker stop [OPTIONS] CONTAINER [CONTAINER...]` | 停止一个或多个容器 |
| `docker start [OPTIONS] CONTAINER [CONTAINER...]` | 启动一个或多个已经被停止的容器 |
| `docker restart [OPTIONS] CONTAINER [CONTAINER...]` | 重启一个或多个容器 |
| `docker rm [OPTIONS] CONTAINER [CONTAINER...]` | 删除一个或多个容器 |
| `docker rmi [OPTIONS] IMAGE [IMAGE...]` | 删除一个或多个镜像 |
| `docker exec [OPTIONS] CONTAINER COMMAND [ARG...]` | 在运行的容器中执行命令 |
| `docker-compose up [options]` | 创建并启动 Docker 容器服务 |
| `docker-compose down [options]` | 停止并移除 Docker 容器服务 |
| `docker network ls` | 列出 Docker 网络 |
| `docker volume ls` | 列出 Docker 数据卷 |
| `docker stats [OPTIONS] [CONTAINER...]` | 显示容器资源使用情况统计信息 |
| `docker info [OPTIONS]` | 显示 Docker 系统信息 |
| `docker inspect [OPTIONS] NAME\|ID [NAME\|ID...]` | 获取容器或镜像的详细信息 |
| `docker logs [OPTIONS] CONTAINER` | 获取容器的日志输出 |
| `docker cp [OPTIONS] CONTAINER:SRC_PATH DEST_PATH\|` | 从容器中复制文件/目录到主机 |
| `docker-compose logs [options]` | 查看 Docker 容器服务的日志输出 |
| `docker-compose exec [options] SERVICE COMMAND [ARGS...]` | 在容器服务中执行命令 |

## `docker run`
```c
# 运行一个容器
docker run -it -p 8088:8088 -p 8089:8089 -p 8090:9090 -v /root/soft/docker:/root/soft/docker -v /root/soft/dockertt:/root/soft/dockertt loen/rc /bin/bash
命令的格式：
Usage: docker run [OPTIONS] IMAGE [COMMAND] [ARG...]
-a, --attach=[] 登录容器（以docker run -d启动的容器）
-c, --cpu-shares=0 设置容器CPU权重，在CPU共享场景使用
--cap-add=[] 添加权限，权限清单详见：http://linux.die.net/man/7/capabilities
--cap-drop=[] 删除权限，权限清单详见：http://linux.die.net/man/7/capabilities
--cidfile="" 运行容器后，在指定文件中写入容器PID值，一种典型的监控系统用法
--cpuset="" 设置容器可以使用哪些CPU，此参数可以用来容器独占CPU
-d, --detach=false 指定容器运行于前台还是后台
--device=[] 添加主机设备给容器，相当于设备直通
--dns=[] 指定容器的dns服务器
--dns-search=[] 指定容器的dns搜索域名，写入到容器的/etc/resolv.conf文件
-e, --env=[] 指定环境变量，容器中可以使用该环境变量
--entrypoint="" 覆盖image的入口点
--env-file=[] 指定环境变量文件，文件格式为每行一个环境变量
--expose=[] 指定容器暴露的端口，即修改镜像的暴露端口
-h, --hostname="" 指定容器的主机名
-i, --interactive=false 打开STDIN，用于控制台交互
--link=[] 指定容器间的关联，使用其他容器的IP、env等信息
--lxc-conf=[] 指定容器的配置文件，只有在指定--exec-driver=lxc时使用
-m, --memory="" 指定容器的内存上限
--name="" 指定容器名字，后续可以通过名字进行容器管理，links特性需要使用名字
--net="bridge" 容器网络设置，待详述
-P, --publish-all=false 指定容器暴露的端口，待详述
-p, --publish=[] 指定容器暴露的端口，待详述
--privileged=false 指定容器是否为特权容器，特权容器拥有所有的capabilities
--restart="" 指定容器停止后的重启策略，待详述
--rm=false 指定容器停止后自动删除容器(不支持以docker run -d启动的容器)
--sig-proxy=true 设置由代理接受并处理信号，但是SIGCHLD、SIGSTOP和SIGKILL不能被代理
-t, --tty=false 分配tty设备，该可以支持终端登录
-u, --user="" 指定容器的用户
-v, --volume=[] 给容器挂载存储卷，挂载到容器的某个目录
--volumes-from=[] 给容器挂载其他容器上的卷，挂载到容器的某个目录
-w, --workdir="" 指定容器的工作目录
>>>>>> 详细讲解
端口暴露
-P参数：docker自动映射暴露端口；
docker run -d -P training/webapp <span style="color:#009900;">//docker自动在host上打开49000到49900的端口，映射到容器（由镜像指定，或者--expose参数指定）的暴露端口；</span>
-p参数：指定端口或IP进行映射；
docker run -d -p 5000:80 training/webapp <span style="color:#009900;">//host上5000号端口，映射到容器暴露的80端口；</span>
docker run -d -p 127.0.0.1:5000:80 training/webapp <span style="color:#009900;">//host上127.0.0.1:5000号端口，映射到容器暴露的80端口；</span>
docker run -d -p 127.0.0.1::5000 training/webapp <span style="color:#009900;">//host上127.0.0.1:随机端口，映射到容器暴露的80端口；</span>
docker run -d -p 127.0.0.1:5000:5000/udp training/webapp <span style="color:#009900;">//绑定udp端口；</span>
网络配置
--net=bridge： <span style="color:#009900;">//使用docker daemon指定的网桥</span>
--net=host： <span style="color:#009900;">//容器使用主机的网络</span>
--net=container:NAME_or_ID：<span style="color:#009900;">//使用其他容器的网路，共享IP和PORT等网络资源</span>
--net=none： <span style="color:#009900;">//容器使用自己的网络（类似--net=bridge），但是不进行配置</span>
```

## `docker stop`
```bash
# 关闭运行中的容器
docker stop 容器ID
```

## `docker start`
```bash
# 启动一个已经停止的容器
docker start 容器ID
# 重启一个容器
docker restart 容器ID
```

## `docker attach`
```bash
# 进入一个运行中的容器
docker attach 容器ID
```


## `docker ps`
```bash
# 显示全部容器
docker ps -a
# 显示当前运行的容器
docker ps
```

## `docker images`
```bash
# 查看本地镜像
docker images
```

## `docker rmi`
```bash
# 删除所有镜像
docker rmi $(docker images | grep -v RESPOSITORY | awk '{print $3}')
```

## `docker build`
```bash
# 构建容器
docker build -t 镜像名称 .     # 后面的. 指的是当前文件夹 (其实是Dockerfile存放的文件夹)
# 建立映像文件。–rm 选项是告诉Docker，在构建完成后删除临时的Container，Dockerfile的每一行指令都会创建一个临时的Container，一般这些临时生成的Container是不需要的
docker build --rm=true -t loen/lamp .
```

## `docker rm` 
```bash
# 删除容器
docker rm 容器ID
# 删除所有容器
docker rm $(docker ps -a) 
```

## `docker history`
```bash
# 查看历史
docker history 镜像ID
```

## `docker export`
```bash
# 导出容器
docker export 容器ID > xxx.tar
```

## `docker save` 
```bash
# 把 mynewimage 镜像保存成 tar 文件
docker save myimage | bzip2 -9 -c> /home/save.tar.bz2
```

## `docker load`
```bash
# 加载 myimage 镜像
bzip2 -d -c < /home/save.tar.bz2 | docker load
```

## `docker port`
```bash
# 给容器映射一个端口
docker port a581df505cb9 22
49153
```

docker常用运维命令总结：在centos中一般通过`systemd`启动与管理`docker`：

1. 启动docker：`sudo systemctl start docker`

2. 关闭docker：`sudo systemctl stop docker`

3. docker开机自启：`sudo systemctl enable docker`

4. 查看docker日志： `journalctl -u docker.service or less /var/log/messages | grep Docker`

5. 查看服务运行状态：`systemctl status docker.service`

6. systemd启动docker.service逻辑：`cat /usr/lib/systemd/system/docker.service`

7. docker数据存储目录：`tree -L 1 /var/lib/docker`

8. 删除docker数据存储目录：`rm -rf /var/lib/docker/ or docker system prune -a or docker volume rm $(docker volume ls -q) 删除所有卷`

9. 查看docker所占磁盘空间：`cd /var/lib/docker && du -sh * or docker system df`

10. docker磁盘挂载信息：`mount | grep overlay2`

11. docker配置信息：`ls  /etc/docker`

12. 理解容器内外进程id的关联信息：
       容器内：`docker exec etcd0 ps -ef`   
       容器外：`docker top etcd0 关联pid信息 pstree -pl | grep docker`
13. 删除所有容器：docker rm -f  `docker ps -a -q`
14. 运行某一个容器：`docker run  -it -d -p 6379:6379  --name mx-redis  mx/redis:1.0`

## 查看容器 IP 地址

```bash
# 通过容器名称或ID查看IP
sudo docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' <container_name_or_id>

# 例如，查看名为 "zookeeper" 的容器的IP地址
sudo docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' zookeeper
```

## 查看容器日志

```bash
# 查看指定容器的实时日志
sudo docker logs -f <container_name_or_id>

# 例如，查看名为 "kafka" 的容器的实时日志
sudo docker logs -f kafka

# 查看指定容器的最近日志（最后几行）
sudo docker logs <container_name_or_id>

# 例如，查看名为 "zookeeper" 的容器的最近日志
sudo docker logs zookeeper
```

## 使用 docker-compose 查看容器 IP 地址和日志

```bash
# 查看容器 IP 地址
sudo docker-compose exec <service_name> hostname -i

# 例如，查看名为 "zookeeper" 的服务的容器IP地址
sudo docker-compose exec zookeeper hostname -i

# 查看容器日志
sudo docker-compose logs -f <service_name>

# 例如，查看名为 "kafka" 的服务的实时日志
sudo docker-compose logs -f kafka
```

# Docker Compose常用命令

以下是 Docker Compose 常用命令：

| 命令                                       | 描述                               |
|--------------------------------------------|------------------------------------|
| `docker-compose up`                        | 启动应用程序                       |
| `docker-compose up -d`                     | 启动应用程序，并在后台运行         |
| `docker-compose down`                      | 停止应用程序                       |
| `docker-compose down --volumes`            | 停止并删除容器、网络和数据卷       |
| `docker-compose ps`                        | 查看容器运行状态                   |
| `docker-compose logs`                      | 查看容器的日志输出                 |
| `docker-compose up --build`                | 重建容器                           |
| `docker-compose version`                   | 查看 Docker Compose 版本           |
| `docker-compose config`                    | 检查配置                           |
| `docker-compose --help`                    | 查看 Docker Compose 帮助信息       |

示例：
```bash
# 在当前目录下启动应用程序
docker-compose up

# 后台运行应用程序
docker-compose up -d

# 停止应用程序
docker-compose down

# 重建并启动应用程序
docker-compose up --build
```

这些命令可用于管理 Docker Compose 定义的多容器应用程序。
