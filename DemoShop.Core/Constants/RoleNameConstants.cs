namespace DemoShop.Core.Constants
{
    public static class RoleNameConstants
    {
        //Модуль администрирования
        public const string UserManagement = "UserManagement"; //Управление пользователями

        public const string RoleManagment = "RoleManagment"; //Управление ролями
        public const string RoleManagmentDelete = "RoleManagmentDelete"; //Удаление ролей

        //Модуль товаров
        public const string UnitManagment = "UnitManagment"; //Управление единицами измерений
        public const string ProductsCategoryManagment = "ProductsCategoryManagment"; //Управление категориями продуктов
        public const string ProductManagment = "ProductManagment"; //Управление товарами

        //Модуль покупок
        public const string SalesInvoiceManagment = "SalesInvoiceManagment"; //Управление покупками

        //Модуль продаж
        public const string PurchaseInvoiceManagment = "PurchaseInvoiceManagment"; //Управление прожадами
    }
}