using POSService;
using POSService.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace BarMateTabletOrdering.Security
{
    public class CustomRoleProvider : RoleProvider
    {

        protected static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Core"].ConnectionString;
        }

        private IEnumerable<CabbashUser> ReadPersonItems(SqlDataReader reader)
        {
            while (reader.Read())
            {
                yield return new CabbashUser
                {
                    PersonID = reader.GetInt32(reader.GetOrdinal("PersonID")),
                    PersonTypeId = reader.GetInt32(reader.GetOrdinal("PersonTypeId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    PersonTypeName = reader.GetString(reader.GetOrdinal("PersonTypeName"))
                };
            };
        }


        public IEnumerable<CabbashUser> GetPersons(int personId)
        {
            using (SqlConnection myConnection = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllUsers", myConnection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    myConnection.Open();

                    cmd.Parameters.AddWithValue("@Id", personId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    return ReadPersonItems(reader).ToArray();

                }
            }

        }


        public CustomRoleProvider()
        {
           
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var allPersonsA = GetPersons(0);

            var allPersons = allPersonsA.ToList();

            var person = allPersons.FirstOrDefault(x => x.PersonID.ToString().Equals(username));

            if (person == null)
                return false;

            if (person.PersonTypeName.Equals(roleName, StringComparison.CurrentCultureIgnoreCase))
                return true;

            return false;
        }

        public override string[] GetRolesForUser(string username)
        {
            int personId = 0;

            int.TryParse(username, out personId);

            var allPersonsA = GetPersons(0);

            var allPersons = allPersonsA.ToList();

            var person = allPersons.FirstOrDefault(x => x.PersonID == personId);// POSService.StockItemService.GetUserById(personId).FirstOrDefault();

            if (person != null)
            {
                var role = person.PersonTypeName;
                return new string[] { person.PersonTypeName };
            }

                

            return null;

        }

        // -- Snip --
        public override string[] GetAllRoles()
        {
            return new string[] { "Admin", "Staff", "Guest" };
        }
        // -- Snip --

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}