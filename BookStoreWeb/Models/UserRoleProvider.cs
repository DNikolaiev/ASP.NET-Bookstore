using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
namespace BookStoreWeb.Models
{
    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (string rolename in roleNames)
            {
                if (rolename == null || rolename == "")
                    throw new System.Configuration.Provider.ProviderException("Role name cannot be empty or null.");
                if (!RoleExists(rolename))
                    throw new System.Configuration.Provider.ProviderException("Role name not found.");
            }
            foreach (string username in usernames)
            {
                if (username == null || username == "")
                    throw new System.Configuration.Provider.ProviderException("User name cannot be empty or null.");
                if (username.Contains(","))
                    throw new ArgumentException("User names cannot contain commas.");

                foreach (string rolename in roleNames)
                {
                    if (IsUserInRole(username, rolename))
                        throw new System.Configuration.Provider.ProviderException("User is already in role.");
                }
            }
            try
            {
                using (var db = new BookStoreContext())
                {
                    foreach (string username in usernames)
                    {
                        foreach (string role in roleNames)
                        {

                            int role_id = db.Roles.Where(r => r.Role == role).FirstOrDefault().Id;
                            db.UserInRole.Add(new UserRoleMapping
                            {
                                RoleId = role_id,
                                UserName = username
                            });

                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception) { return;  }
        }

        public override void CreateRole(string roleName)
        {
            using (var db = new BookStoreContext())
            {
                db.Roles.Add(new RoleMaster { Role = roleName });
                db.SaveChanges();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (var db = new BookStoreContext())
            {
                try
                {
                    db.Roles.Remove(db.Roles.Where(x => x.Role == roleName).First());
                    db.SaveChanges();
                    return true;
                }
                catch (Exception) { return false; }
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (var db = new BookStoreContext())
            {
                return db.Roles.Select(x => x.Role).ToArray<string>();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var db = new BookStoreContext())
            {
                List<string> roles = new List<string>();
                var query =
                    from map in db.UserInRole
                    join u in db.Users
                    on map.UserName equals u.UserName
                    join r in db.Roles
                    on map.RoleId equals r.Id
                    where u.UserName == username
                    select r.Role;
                foreach(var item in query)
                {
                    roles.Add(item);
                }
                return roles.ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (var db = new BookStoreContext())
            {
                var query =
                    from u in db.Users
                    join map in db.UserInRole
                    on u.UserName equals map.UserName
                    join r in db.Roles
                    on map.RoleId equals r.Id
                    where r.Role == roleName
                    select new { User = u };
                List<string> usersInRole = new List<string>();
                foreach(var entity in query)
                {
                    usersInRole.Add(entity.User.UserName);
                }
                return usersInRole.ToArray();
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {

            List<string> users = GetUsersInRole(roleName).ToList() ?? new List<string> { "-" };
                return users.Contains(username);
            
        }
        //TODO: fix
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var db = new BookStoreContext())
            {
                foreach (string user in usernames)
                {
                    foreach (string role in roleNames)
                    {
                        if (!IsUserInRole(user, role)) continue;
                        var role_id = db.Roles.Where(x => x.Role == role).FirstOrDefault().Id; ;
                        db.UserInRole.Remove(db.UserInRole.Where(x => x.RoleId == role_id && x.UserName==user).FirstOrDefault());
                    }
                }
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (var db = new BookStoreContext())
            {
                return db.Roles.Where(r => r.Role == roleName).FirstOrDefault() == null ? false : true;
            }
        }
    }
}