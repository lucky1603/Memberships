using Memberships.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Memberships.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserFirstName(this IIdentity identity)
        {
            var db = ApplicationDbContext.Create();
            var user = db.Users.FirstOrDefault(u => u.UserName.Equals(identity.Name));
            return user != null ? user.FirstName : string.Empty;
        }
    
        public static async Task GetUsers(this List<UserModel> users)
        {
            var db = ApplicationDbContext.Create();
            var list = await (from u in db.Users
                              select new UserModel
                              {
                                  FirstName = u.FirstName,
                                  Id = u.Id,
                                  Email = u.Email
                              }).OrderBy(o => o.Email).ToListAsync();

            users.AddRange(list);
        }
    }
}