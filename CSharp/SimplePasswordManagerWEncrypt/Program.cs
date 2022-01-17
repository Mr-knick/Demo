using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;


namespace Password_Manager
{
    class Program
    {
        public class Servers
        {
            public string ServerName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

            //public void CopyServer(Servers oldserver)
            //{
            //    ServerName = oldserver.ServerName;
            //    UserName = oldserver.UserName;
            //    Password = oldserver.Password;
            //}

            public void GetServerOrChangePassword()
            {
                Console.WriteLine("\nServer Name: " + ServerName);
                Console.WriteLine("UserName: " + UserName);
                Console.WriteLine("Password: " + Password);

                while (true)
                {
                    Console.WriteLine("Would you like to change the username or password? y/(n) ");
                    String UserInput = Console.ReadLine();
                    if (UserInput == "n" || string.IsNullOrWhiteSpace(UserInput))
                    {
                        return;
                    }
                    else if (UserInput == "y" )
                    {
                        while (true)
                        {
                            Console.WriteLine("Would you like to change the username or password? username/(password) ");
                            String UserInput2 = Console.ReadLine();
                            if (UserInput2 == "password" || string.IsNullOrWhiteSpace(UserInput2))
                            {
                                NewPassword();
                                return;
                            }
                            else if (UserInput2 == "username")
                            {
                                AddUserName();
                                return;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Input");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input");
                    }
                }

            }
            public void CreateServer(string UserServerName)
            {
                while (true)
                {
                    Console.WriteLine(UserServerName + " does not exist. Would you like to create it? (y)/n ");
                    String UserInput = Console.ReadLine();
                    if (UserInput == "y" || string.IsNullOrWhiteSpace(UserInput))
                    {
                        CreateServerOnly(UserServerName);
                        AddUserName();
                        NewPassword();
                        ServerDictionary.Add(this.ServerName, this);
                        IsDicAndDatabaseInSync = false;
                        logevents("Server created");
                        return;
                    }
                    else if (UserInput == "n")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input");
                    }
                }
            }
            public void CreateServerOnly(string NewServerName)
            {
                ServerName = NewServerName;
                UserName = null;
                Password = null;
                IsDicAndDatabaseInSync = false;
            }
            public void AddUserName()
            {
                while (true)
                {
                    Console.WriteLine("Please enter a username for server: ");
                    String UserInput = Console.ReadLine();
                    if (UserInput == null)
                    {
                        Console.WriteLine("No username set please try again");
                    }
                    else 
                    {
                        Console.WriteLine("Confirm use of <" + UserInput + "> (y)/n: ");
                        String UserInput2 = Console.ReadLine();
                        if (UserInput2 == "y" || string.IsNullOrWhiteSpace(UserInput2))
                        {
                            this.UserName = UserInput;
                            IsDicAndDatabaseInSync = false;
                            logevents("Username modified");
                            return;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            public void NewServerAll(string NewServerName, string NewUserName, string NewPassword)
            {
                ServerName = NewServerName;
                UserName = NewUserName;
                Password = NewPassword;
                IsDicAndDatabaseInSync = false;
            }
            public void NewPassword()
            {
                while (true)
                {
                    Console.WriteLine("Pleases enter a unique password: "); 
                    String UserInput = Console.ReadLine();
                    if (UserInput == null)
                    {
                        Console.WriteLine("No password set please try again");
                    }
                    else
                    {
                        bool ValidPassword = true;
                        foreach (Servers s in ServerDictionary.Values)
                        {
                            if (s.Password == UserInput)
                            {
                                Console.WriteLine("Password already in use please try again: ");
                                ValidPassword = false;
                                break;
                            }
                        }
                        if (ValidPassword)
                        {
                            Console.WriteLine("Confirm use of <" + UserInput + "> (y)/n: ");
                            String UserInput2 = Console.ReadLine();
                            if (UserInput2 == "y" || string.IsNullOrWhiteSpace(UserInput2))
                            {
                                this.Password = UserInput;
                                IsDicAndDatabaseInSync = false;
                                logevents("Password modified");
                                return;
                            }
                        }

                    }
                }
            }
        }
        static Dictionary<string, Servers> ServerDictionary = new Dictionary<string, Servers>();
        static Boolean IsDicAndDatabaseInSync = true;
        static string databasepassword = null;

        static public void logevents(string text)
        {
            string path = Directory.GetCurrentDirectory();
            string logfile = "\\logfile.log";
            string pathandlog = path + logfile;
            string time = DateTime.Now.ToString("h:mm:ss tt");
            File.AppendAllText(pathandlog, time + " " + text);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to Database");

            string path = Directory.GetCurrentDirectory();
            string databasefile = "\\database.db";
            string PathAndDataBase = path + databasefile;
            string database = null;

            while (true)
            {
                Console.WriteLine("Please enter database password: ");
                string UserInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(UserInput))
                {
                    Console.WriteLine("Invalid. Please Try again.");
                }
                else
                {
                    databasepassword = UserInput;
                    Console.WriteLine("Connecting to database with password of <" + databasepassword + ">");
                    logevents("Database loging attempt");
                    break;
                }
            }

            List<string> eachelementofdatabase = new List<string>();
            void SyncDictionaryAndDatabase()
            {
                try
                {
                    database = null;

                    if (ServerDictionary.Count > 0)
                    {
                        foreach (Servers s in ServerDictionary.Values)
                        {
                            database += s.ServerName;
                            database += "\n";
                            database += s.UserName;
                            database += "\n";
                            database += s.Password;
                            database += "\n";
                        }
                    }

                    byte[] rawPlaintext = System.Text.Encoding.Unicode.GetBytes(database);

                    using (Aes aes = new AesManaged())
                    {
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 128;          // in bits
                        var md5 = new MD5CryptoServiceProvider();
                        var md5data = md5.ComputeHash(Encoding.ASCII.GetBytes(databasepassword));

                        aes.Key = md5data;
                        aes.IV = md5data;
                        byte[] cipherText = null;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(rawPlaintext, 0, rawPlaintext.Length);
                            }

                            cipherText = ms.ToArray();
                        }
                        File.WriteAllBytes(PathAndDataBase, cipherText);
                    }
                        IsDicAndDatabaseInSync = true;
                }
                catch
                {
                    Console.WriteLine("Sync with database failed. Please restart application and ensure database file isn't open by another application.");
                    logevents("Failed Database sync");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }

            if (File.Exists(PathAndDataBase))
            {
                Console.WriteLine("Loading database");

                try
                {
                    using (Aes aes = new AesManaged())
                    {
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 128;          // in bits
                        var md5 = new MD5CryptoServiceProvider();
                        var md5data = md5.ComputeHash(Encoding.ASCII.GetBytes(databasepassword));

                        aes.Key = md5data;
                        aes.IV = md5data;
                        byte[] cipherText = null;
                        byte[] plainText = null;

                        cipherText = File.ReadAllBytes(PathAndDataBase);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherText, 0, cipherText.Length);
                            }

                            plainText = ms.ToArray();
                        }
                        database = System.Text.Encoding.Unicode.GetString(plainText);
                    }
                    using (var reader = new StringReader(database))
                    {
                        for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                        {
                            eachelementofdatabase.Add(line);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to load database please check password");
                    logevents("Failed Database read");
                    Environment.Exit(0);
                }
                for (int i = 0; i < eachelementofdatabase.Count; i += 3)
                {
                    Servers tempserver = new Servers();
                    tempserver.ServerName = eachelementofdatabase[i];
                    tempserver.UserName = eachelementofdatabase[i + 1];
                    tempserver.Password = eachelementofdatabase[i + 2];
                    ServerDictionary.Add(tempserver.ServerName, tempserver);
                }
            }

            String UserInputString = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please input server name, <exit> to leave application, or <remove> to delete a server: ");
                UserInputString = Console.ReadLine();
                if (UserInputString == "exit")
                {
                    Environment.Exit(0);
                }
                else if (UserInputString == "remove")
                {
                    Console.WriteLine("Enter Server Name");
                    String UserInput = Console.ReadLine();
                    if (ServerDictionary.ContainsKey(UserInput))
                    {
                        ServerDictionary.Remove(UserInput);
                        IsDicAndDatabaseInSync = false;
                        logevents("Server Removed");
                    }
                    else
                    {
                        Console.WriteLine("<" + UserInputString + "> not found returning to main menu.");
                        Console.ReadLine();
                    }
                }
                else if (ServerDictionary.ContainsKey(UserInputString))
                {
                    ServerDictionary[UserInputString].GetServerOrChangePassword();
                }
                else if (string.IsNullOrWhiteSpace(UserInputString))
                {
                    //do nothing
                }
                else
                {
                    Servers BuildNewServer = new Servers();
                    BuildNewServer.CreateServer(UserInputString);
                }
                if (!IsDicAndDatabaseInSync)
                {
                    Console.WriteLine("Syncing with database");
                    SyncDictionaryAndDatabase();
                }
            }
        }
    }
}

