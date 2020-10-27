using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DomainTest
{

    public class ObservableCollectionC<T> : ObservableCollection<T>
    {
        public ObservableCollectionC()
        {

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var root = new DirectoryEntry("LDAP://RootDSE");
                //Console.WriteLine( root.Properties["defaultNamingContext"][0]);
                var users = new List<StaffMember>();
                try
                {
                    //var root = new DirectoryEntry("LDAP://RootDSE", "enpsserveradmin", "zipdisk");
                    //string str = root.Properties["defaultNamingContext"][0].ToString();
                  var  root = new DirectoryEntry("LDAP://usviking.com", "enpsserveradmin", "zipdisk");
                    var search = new DirectorySearcher(root);
                    search.PageSize = 1000; // we need to set DirectorySearcher.PageSize to a non-zero value to get all results since the default is 1000 records
                    search.Filter = "(&(objectClass=user)(objectCategory=person))";
                    search.PropertiesToLoad.Add("samaccountname");
                    search.PropertiesToLoad.Add("displayname");
                    search.PropertiesToLoad.Add("givenName");
                    search.PropertiesToLoad.Add("sn");
                    var results = search.FindAll();
                    if (results != null)
                    {
                        foreach (SearchResult result in results)
                        {
                            var user = new StaffMember();
                            if (result.Properties.Contains("samaccountname"))
                                user.UserID = (string)result.Properties["samaccountname"][0];
                            if (result.Properties.Contains("displayname"))
                                user.DisplayName = (string)result.Properties["displayname"][0];
                            else
                                user.DisplayName = "AD Import";
                            if (result.Properties.Contains("givenName"))
                                user.GivenName = (string)result.Properties["givenName"][0];
                            else
                                user.GivenName = "AD Import";
                            if (result.Properties.Contains("sn"))
                                user.Surname = (string)result.Properties["sn"][0];
                            else
                                user.Surname = "AD Import";
                            Console.WriteLine("Users cout: {0}", users.Count);
                            users.Add(user);
                        }
                    }
                    results.Dispose(); // we dispose the SearchResultCollection returned by FindAll, otherwise we may have a memory leak
 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }  
          

            Console.ReadKey();
        }

        private static bool IsCertificateInstalled()
        {
            //string setEnpsINIFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SETENPS.INI");
            //INIFile setEnpsINIFile = new INIFile(setEnpsINIFilePath);
            //var certPath = string.Format("{0}:\\Nom\\NOMCert.pfx", setEnpsINIFile.Read("Data", "Volumes"));

            var certPath = @"C:\Work\ENPS\DEV\v8.3\Server\NOM\APPlatformCert.pfx";
            try
            {
                if (File.Exists(certPath))
                {
                    var certificate = new X509Certificate2(certPath, "zipdisk", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                    var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite);
                    store.Remove(certificate);
                    store.Close();
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
