# Net5.WebApi项目介绍：

1. 使用net5写的webapi框架
2. 调用`百度身份证识别`接口，做身份证信息读取
3. 调用`企业微信`接口获取accesstoken用户获取企业微信信息
4. 调用`飞鸽`接口做手机短信验证功能
5. 使用IMemoryCash做token等缓存
6. 使用JWT做用户角色权限控制（demo，未实际使用）
7. 配置了跨域解决方法
8. 配置并使用sqlsugar数据库，及DBFirst
9. 配置了Nlog用于记录服务运行状态等
10. 配置了Swagger api文档
11. 使用ActionFilter/ExceptionFilter/ResultFilter实现AOP
12. 使用hangfire写后台job，实现定时任务（电子合同发起及下载等）
13. 实现自定义模型验证功能
14. 配置appsettings实例使用方式，简化配置的调用修改

