using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plataforma.Data;
using Plataforma.Extensions;
using Plataforma.Models.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Services.Components.Repositories;
public class UsersRepository {
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsersRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User> GetUserByIdAsync(int id, bool onlyActive = true) {
        return await GetQueryableUsers(onlyActive: onlyActive).WithNoLockFirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<User> GetUserByEmailAsync(string email, bool onlyActive = true) {
        return await GetQueryableUsers(onlyActive: onlyActive).WithNoLockFirstOrDefaultAsync(u => u.Email == email);
    }
    public IQueryable<User> GetQueryableUsers(bool onlyActive = true) {
        var showMaster = _httpContextAccessor?.HttpContext?.User?.IsMaster() ?? false;
        var users = _dbContext.Users.AsQueryable();
        if (onlyActive)
            users = users.Where(u => u.Active);
        if (!showMaster)
            users = users.Where(u => !u.Master);
        return users.OrderBy(g => g.Name);
    }


    public async Task<SelectList> GetUsersSelectListAsync(bool onlyActive = true) {
        var users = await GetQueryableUsers(onlyActive).WithNoLockToListAsync();
        return new SelectList(users.Select(g =>
            new {
                Id = g.Id,
                Name = g.Name
            }), "Id", "Name");
    }
}
