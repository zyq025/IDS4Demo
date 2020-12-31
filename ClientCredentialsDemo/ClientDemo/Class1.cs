/*static async Task Main(string[] args)
{
    // 1. 创建一个HttpClient用于请求
    var client = new HttpClient();
    // 2. 获取授权服务器的相关信息，IdentityModel已经将其封装好了
    var disco = await client.GetDiscoveryDocumentAsync("http://localhost:6100");
    // 3. 检查是否请求错误
    if (disco.IsError)
    {
        // 错误就打印错误信息，然后直接返回
        Console.WriteLine(disco.Error);
        return;
    }
    // 4. 通过授权服务分配的标识，向授权服务器请求AccessToken
    var tokenResp = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        // 指定获取token的地址，IdentityModel进行封装，直接使用即可
        Address = disco.TokenEndpoint,
        // 指定授权服务器分配的客户端标识
        ClientId = "client",
        // 指定授权服务器分的客户端密码
        ClientSecret = "ordersecret"
    });
    // 5. 检查获取Token是否成功
    if (tokenResp.IsError)
    {
        // 如果失败，打印错误消息并返回
        Console.WriteLine(tokenResp.Error);
        return;
    }

    // 6. 创建一个请求API资源的HttpClient
    var apiClient = new HttpClient();
    // 7. 将获取到的Token以Bearer的方案设置在请求头中
    apiClient.SetBearerToken(tokenResp.AccessToken);
    // 8. 向资源服务器中请求受保护的API
    var contentResp = await apiClient.GetAsync("http://localhost:5000/api/Order");
    // 9. 打印对应的消息
    if (contentResp.IsSuccessStatusCode)
    {
        var content = await contentResp.Content.ReadAsStringAsync();
        Console.WriteLine(JArray.Parse(content));
    }
    else
    {
        Console.WriteLine(contentResp.StatusCode);
    }

    Console.ReadLine();
}*/