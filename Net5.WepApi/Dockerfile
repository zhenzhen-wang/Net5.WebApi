#基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:5.0
#将发布好的程式拷贝到app目录，published为服务器实体路径，app为docker容器内路径
COPY published/ app/
#设定工作目录,不需要事先创建,没有则会自动新增
WORKDIR /app
#声明端口号，需要与生产环境指定端口一致
EXPOSE 5051
#运行程式
ENTRYPOINT ["dotnet", "Mitac.Net5.WepApi.dll"] 

#必须设置，否则读取不到数据库数据
ENV TZ=Asia/Shanghai  
#更改容器编码，防止修改appsettings时，中文乱码
ENV LANG C.UTF-8