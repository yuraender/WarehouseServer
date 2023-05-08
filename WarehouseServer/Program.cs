using DevOne.Security.Cryptography.BCrypt;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WarehouseServer.Properties;
using WarehouseServer.Server;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Sql;

namespace WarehouseServer {
    public class Program {

        private static readonly string _logFile =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            + $"/WarehouseServer/log-{DateTime.Now:dd-MM-yyyy-HH-mm-ss}.txt";

        public static Server.WarehouseServer Server {
            get; set;
        }

        public static void Main(string[] args) {
            StreamWriter writer = new StreamWriter(new FileStream(_logFile, FileMode.Create));
            writer.AutoFlush = true;
            Console.SetOut(writer);
            Console.SetError(writer);

            double unix = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            Server = new Server.WarehouseServer(5490, new ServerHandler());
            Server.Run();
            Log($"Starting WarehouseServer on {Environment.MachineName} with PID {Process.GetCurrentProcess().Id}");
            RunTrayIcon();

            Log("EntityFramework - Starting...");
            try {
                using (ServerContext context = new ServerContext()) {
                    if (context.Users.Count() == 0) {
                        User user = new User {
                            Name = "Администратор", Login = "admin",
                            Password = BCryptHelper.HashPassword("admin", BCryptHelper.GenerateSalt()),
                            IsAdmin = true
                        };
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                    context.Database.ExecuteSqlCommand(
                        "ALTER TABLE Users DROP CONSTRAINT [FK_dbo.Users_dbo.Units_Unit_ID];");
                    context.Database.ExecuteSqlCommand(
                        @"ALTER TABLE Users ADD CONSTRAINT [FK_dbo.Users_dbo.Units_Unit_ID]
                          FOREIGN KEY (Unit_ID) REFERENCES Units (ID)
                          ON DELETE SET NULL ON UPDATE NO ACTION;");
                }
                Log("EntityFramework - Start completed.");
            } catch (Exception ex) {
                Log($"EntityFramework - {ex.Message}");
                MessageBox.Show("Не удается подключиться к базе данных.");
                Application.Run(new ChangeDatabase());
            }

            double seconds = (DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds - unix) / 1000.0D;
            Log($"Started WarehouseServer in {seconds:0.000} seconds");
        }

        public static void Log(string message) {
            Console.WriteLine($"{DateTime.Now} | {message}");
        }

        private static void RunTrayIcon() {
            Thread notifyThread = new Thread(() => {
                ContextMenu menu = new ContextMenu();
                NotifyIcon notificationIcon = new NotifyIcon() {
                    Icon = Resources.Warehouse_icon,
                    ContextMenu = menu,
                    Text = "Warehouse Server",
                    Visible = true
                };
                MenuItem menuLog = new MenuItem("Open Logfile");
                menuLog.Click += (sender, e) => {
                    Process.Start("notepad.exe", _logFile);
                };
                MenuItem menuConfig = new MenuItem("Edit Configfile");
                menuConfig.Click += (sender, e) => {
                    if (Application.OpenForms.Count == 0) {
                        new ChangeDatabase().Show();
                    }
                };
                MenuItem menuStop = new MenuItem("Stop Server");
                menuStop.Click += (sender, e) => {
                    notificationIcon.Dispose();
                    Environment.Exit(0);
                };
                menu.MenuItems.Add(0, menuLog);
                menu.MenuItems.Add(1, menuConfig);
                menu.MenuItems.Add(2, new MenuItem("-"));
                menu.MenuItems.Add(3, menuStop);
                Application.Run();
            });
            notifyThread.Start();
        }
    }
}
