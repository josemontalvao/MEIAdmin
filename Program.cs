using MEIAdmin.Data;
using Microsoft.EntityFrameworkCore;
using System;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configuração do banco de dados MySQL
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Connection String: {connectionString}");

    Console.WriteLine("Antes de AddDbContext...");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        try
        {
            Console.WriteLine("Dentro do AddDbContext...");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            Console.WriteLine("DbContext configurado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao configurar o DbContext: {ex.Message}");
            throw;
        }
    });
    Console.WriteLine("Depois de AddDbContext...");

    // Adiciona serviços MVC ao projeto
    Console.WriteLine("Adicionando serviços MVC...");
    builder.Services.AddControllersWithViews();
    Console.WriteLine("Serviços MVC adicionados com sucesso!");

    // ATIVAÇÃO DE SESSÃO SEGURA PARA O APP DO CELULAR (12 HORAS DE DURAÇÃO)
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(12);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    var app = builder.Build();

    // Middleware de erros
    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine("Em app.Environment.IsDevelopment()...");
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    // ATIVA O USO DE SESSÃO
    app.UseSession();

    app.UseAuthorization();

    // A PORTA DE ENTRADA DO SISTEMA AGORA É O LOGIN OBRIGATÓRIO!
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Tecnico}/{action=Login}/{id?}");

    Console.WriteLine("Iniciando o aplicativo...");
    app.Run();
    Console.WriteLine("Aplicativo iniciado com sucesso!");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro global não tratado: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

