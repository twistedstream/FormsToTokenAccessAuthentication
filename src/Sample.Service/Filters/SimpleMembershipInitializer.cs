using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Security;
using TS.FormsToTokenAccessAuthentication.Sample.Service.Models;
using WebMatrix.WebData;

namespace TS.FormsToTokenAccessAuthentication.Sample.Service.Filters
{
    public class SimpleMembershipInitializer
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public static void Initialize()
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        public SimpleMembershipInitializer()
        {
            try
            {
                Database.SetInitializer<UsersContext>(null);
                using (var context = new UsersContext())
                {
                    if (!context.Database.Exists())
                    {
                        // Create the SimpleMembership database without Entity Framework migration schema
                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                    }
                } 
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", true);

                // add default roles
                if (!Roles.RoleExists("admin"))
                    Roles.CreateRole("admin");

                // add default accounts
                AddUser("view-user", "passw0rd");
                AddUser("admin-user", "passw0rd", "admin");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
            }
        }
        
        private static void AddUser(string userName, string password, string role = null)
        {
            if (WebSecurity.UserExists(userName)) return;

            WebSecurity.CreateUserAndAccount(userName, password);

            if (role != null)
                Roles.AddUserToRole(userName, role);
        }
    }
}