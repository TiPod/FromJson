# FromJson

> 版本要求 \>=  .net core 3.1

A Library For ASP.Net Core MVC and ASP.Net Core WebAPI,Binding Parameter of Action From Json Data.

提供直接 ASP.Net Core MVC / ASP.Net Core WebAPI将Json数据绑定到Action参数中的库

## 安装

```
Install-Package FromJson
```
## 使用

 [样例](https://github.com/BigPete-King/FromJson/tree/main/FromJson.Example)

在需要绑定的参数上添加 **[FromJson]**
```csharp
[HttpPost]
public IActionResult Post([FromJson]int id,[FromJson]string title){
    return Ok();
}
```
对应的Body
```json
{
    "id":10,
    "title":"title"
}
```





## FromJson 参数
|  字段名   | 说明  |
|  ----  | ----  |
| propertyName  | 自定义Json中Key名称 默认为**参数的变量名** |
| ignoreCase  | 忽略大小写 默认为**false** |