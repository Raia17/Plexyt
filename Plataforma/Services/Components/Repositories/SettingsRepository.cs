using Plataforma.Data;
using Plataforma.Extensions;
using Plataforma.Models.Structure;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Services.Components.Repositories;
public class SettingsRepository {
    private readonly ApplicationDbContext _dbContext;

    public SettingsRepository(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
    }

    public Settings GetSettings() {
        var settings = _dbContext.Settings.OrderBy(s => s.Id).WithNoLockFirstOrDefault();
        if (settings == null) {
            _dbContext.Settings.Add(new Settings());
            _dbContext.SaveChanges();
        }
        settings = _dbContext.Settings.OrderBy(s => s.Id).WithNoLockFirstOrDefault();
        return settings;
    }

    public async Task<Settings> GetSettingsAsync() {
        var settings = await _dbContext.Settings.OrderBy(s => s.Id).WithNoLockFirstOrDefaultAsync();
        if (settings == null) {
            _dbContext.Settings.Add(new Settings());
            await _dbContext.SaveChangesAsync();
        }
        settings = await _dbContext.Settings.OrderBy(s => s.Id).WithNoLockFirstOrDefaultAsync();
        return settings;
    }
    public async Task<bool> SaveSettingsAsync(Settings settings) {
        var currentSettings = await GetSettingsAsync();

        currentSettings.GlobalSettings = settings.GlobalSettings;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<string> GetRules() {
        var settings = await GetSettingsAsync();
        return settings.UsageRules;
    }
    public async Task SaveRules(string rules) {
        var settings = await GetSettingsAsync();
        settings.UsageRules = rules;
        await _dbContext.SaveChangesAsync();
    }
}
