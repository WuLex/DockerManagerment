using Docker.DotNet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IDockerClient>(
    //new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient()
    new DockerClientConfiguration(new Uri("http://192.168.0.102:2375/")).CreateClient()
    );

//var dockerClient = new DockerClientConfiguration().CreateClient();
//builder.Services.AddSingleton(dockerClient);

var configuration = builder.Configuration;

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
