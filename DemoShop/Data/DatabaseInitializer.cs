using DemoShop.Core.Constants;
using DemoShop.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;

namespace DemoShop.UI.Data
{
    public static class DatabaseInitializer
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            try
            {
                UserManager<ShopUser> userManager = serviceProvider.GetRequiredService<UserManager<ShopUser>>();
                if (!userManager.Users.Any<ShopUser>())
                {
                    var db = serviceProvider.GetRequiredService<ShopDbContext>();
                    //Добавление модулей
                    ShopModule moduleAdmin = new ShopModule { Id = Guid.Parse("70AAA0AE-B4BD-464F-83D3-4910CFEDABC4"), Name = "Администрирование" };
                    ShopModule moduleGoods = new ShopModule { Id = Guid.Parse("9242001A-E0E8-4DE6-BE79-53C62E82B743"), Name = "Продукция" };
                    ShopModule modulePurchases = new ShopModule { Id = Guid.Parse("E0D007DB-668F-4C63-9E70-B01AA5C26816"), Name = "Покупки" };
                    ShopModule moduleSales = new ShopModule { Id = Guid.Parse("FBE273FA-F993-4B89-888F-290DB1EA15DE"), Name = "Продажи" };
                    db.Modules.AddRangeAsync(new ShopModule[] { moduleAdmin, moduleGoods, modulePurchases, moduleSales }).Wait();

                    //Добавление ролей
                    //---moduleAdmin
                    RoleManager<ShopRole> roleManager = serviceProvider.GetRequiredService<RoleManager<ShopRole>>();
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.UserManagement, Description = "Управление пользователями", Module = moduleAdmin }).Wait();
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.RoleManagment, Description = "Управление ролями", Module = moduleAdmin }).Wait();
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.RoleManagmentDelete, Description = "Удаление ролей", Module = moduleAdmin }).Wait();
                    //---moduleGoods
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.UnitManagment, Description = "Управление единицами измерений", Module = moduleGoods }).Wait();
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.ProductsCategoryManagment, Description = "Управление категориями товаров", Module = moduleGoods }).Wait();
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.ProductManagment, Description = "Управление товарами", Module = moduleGoods }).Wait();
                    //---modulePurchases
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.PurchaseInvoiceManagment, Description = "Управление покупками", Module = modulePurchases }).Wait();
                    //---moduleSales
                    roleManager.CreateAsync(new ShopRole() { Name = RoleNameConstants.SalesInvoiceManagment, Description = "Управление продажами", Module = moduleSales }).Wait();

                    //Добавление суперадминистратор
                    var _configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    string adminFullName = _configuration.GetSection("DefaultIdentityData:SuperAdministrator:FullName").Value;
                    string adminEmail = _configuration.GetSection("DefaultIdentityData:SuperAdministrator:Login").Value;
                    string password = _configuration.GetSection("DefaultIdentityData:SuperAdministrator:Password").Value;

                    ShopUser admin = new ShopUser { Email = adminEmail, UserName = adminEmail, FullName = adminFullName, EmailConfirmed = true };
                    IdentityResult result = userManager.CreateAsync(admin, password).Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(admin, RoleNameConstants.UserManagement).Wait();
                        userManager.AddToRoleAsync(admin, RoleNameConstants.RoleManagment).Wait();
                        userManager.AddToRoleAsync(admin, RoleNameConstants.RoleManagmentDelete).Wait();
                        userManager.AddToRoleAsync(admin, RoleNameConstants.UnitManagment).Wait();
                        userManager.AddToRoleAsync(admin, RoleNameConstants.ProductsCategoryManagment).Wait();
                        userManager.AddToRoleAsync(admin, RoleNameConstants.ProductManagment).Wait();
                        userManager.AddToRoleAsync(admin, RoleNameConstants.PurchaseInvoiceManagment).Wait();
                        userManager.AddToRoleAsync(admin, RoleNameConstants.SalesInvoiceManagment).Wait();
                    }

                    Log.Information("Initial filling of data in the database was successfully completed");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while initializing the database data");
            }
        }
    }
}