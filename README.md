# DockerManagerment

## 结果如图：

![img](https://raw.githubusercontent.com/WuLex/UsefulPicture/main/dockerwebmanager/result2.jpg)

![img](https://raw.githubusercontent.com/WuLex/UsefulPicture/main/dockerwebmanager/result3.png)


# 配置和命令
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

