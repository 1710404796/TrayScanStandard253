using TestProto.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.WebHost.UseUrls("http://*:50005"); // 设置监听的端口
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/CaptureImage", () => new QRCodeResult { Data = Enumerable.Range(1, 64).Select(s => (s, $"TestCode{s:000}")).ToDictionary(), ErrorCode = ErrorType.Successed});
app.MapGet("/CaptureImageError", () => new QRCodeResult { Data = [], ErrorCode = ErrorType.CameraError });
app.Run();
