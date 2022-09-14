using Microsoft.Extensions.Configuration;

namespace Plataforma.Services.Components;

public class ConfigurationsService {
    public string Domain { get; }
    public string ContentRootPath { get; }
    public string WwwRootPath { get; }
    public string ErrorEmail { get; }
    public bool RedirectWww { get; }
    public bool RedirectHttps { get; }
    public string Title { get; }
    public string Version { get; }
    public SmtpConfig Smtp { get; }
    public int MaxFileSizeBytes { get; }

    public ConfigurationsService(IConfiguration configuration, string contentRootPath, string wwwRootPath) {
        var configurationSection = configuration.GetSection("Configurations");
        ContentRootPath = contentRootPath;
        WwwRootPath = wwwRootPath;
        Domain = configurationSection.GetValue<string>("Domain");
        ErrorEmail = configurationSection.GetValue<string>("ErrorEmail");
        RedirectWww = configurationSection.GetValue<bool>("RedirectWww");
        RedirectHttps = configurationSection.GetValue<bool>("RedirectHttps");
        Title = configurationSection.GetValue<string>("Title");
        Version = configurationSection.GetValue<string>("Version");
        MaxFileSizeBytes = configurationSection.GetValue<int>("MaxFileSizeMb") * 1024 * 1024;
        Smtp = new SmtpConfig(configurationSection.GetSection("Smtp"));
    }

    public class SmtpConfig {
        public string Host { get; }
        public bool Ssl { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }
        public SmtpConfig(IConfiguration section) {
            Host = section.GetValue<string>("Host");
            Ssl = section.GetValue<bool>("Ssl");
            Port = section.GetValue<int>("Port");
            Username = section.GetValue<string>("Username");
            Password = section.GetValue<string>("Password");
        }
    }
}