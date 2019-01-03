using AutoMapper;
using DemoShop.Core.Constants;
using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using DemoShop.UI.Data;
using DemoShop.UI.Models.Identity.RolesViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoShop.UI.Controllers
{
    [Authorize(Roles = RoleNameConstants.RoleManagment)]
    [Route("[controller]/[action]")]
    public class RolesController : ShopIdentityController
    {
        private readonly RoleManager<ShopRole> _roleManager;
        private readonly UserManager<ShopUser> _userManager;
        private readonly ShopDbContext _shopDB;
        private readonly ILogger _logger;

        public RolesController(
            RoleManager<ShopRole> roleManager,
            UserManager<ShopUser> userManager,
            ShopDbContext shopDB)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _shopDB = shopDB;
            _logger = Log.ForContext<RolesController>();
        }

        public async Task<JsonResult> GetData([FromBody]DataManager dm)
        {
            try
            {
                IEnumerable<ShopRole> data = await _roleManager.Roles.Include(r => r.Module).AsNoTracking().ToListAsync();
                DataOperations operation = new DataOperations();

                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    data = (IEnumerable<ShopRole>)operation.PerformSorting(data, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    data = (IEnumerable<ShopRole>)operation.PerformWhereFilter(data, dm.Where, dm.Where[0].Operator);
                }
                int count = data.Cast<ShopRole>().Count();
                if (dm.Skip != 0)
                {
                    data = (IEnumerable<ShopRole>)operation.PerformSkip(data, dm.Skip);
                }
                if (dm.Take != 0)
                {
                    data = (IEnumerable<ShopRole>)operation.PerformTake(data, dm.Take);
                }
                return Json(new { result = data, count = count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error get roles.");
                throw new ApplicationException("Ошибка получения ролей");
            }

        }

        // GET: Roles
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                StatusMessage = AlertMessage.DeserializeObject(StatusMessage)
            };
            return View(model);

        }

        // GET: Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            return View(await GetEditViewModel(id));
        }

        // GET: Roles/Create
        public async Task<ActionResult> Create()
        {
            var modules = new SelectList(await _shopDB.Modules.ToListAsync(), "Id", "Name");
            CreateViewModel model = new CreateViewModel() { Modules = modules };
            return View(model);
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var role = new ShopRole { Name = model.Name, Description = model.Description, ModuleID = model.ModuleID };

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _logger.Information("Role {@Role} successfully added.", role);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Роль успешно добавлена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.Error("Error {@Error} adding role {@Role}.", result.Errors, role);
                }

                AddErrors(result);
                return View(model);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "There was an error adding {RoleName} role", model.Name);
                return View();
            }
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            return View(await GetEditViewModel(id));
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var role = await _roleManager.FindByIdAsync(model.Id.ToString());
                if (role == null)
                {
                    throw new ApplicationException(UnableLoadRole(model.Id.ToString()));
                }

                if (model.Description == role.Description && model.Name == role.Name && model.ModuleID == role.ModuleID)
                {
                    StatusMessage = (new AlertMessage(AlertStatus.info, "Изменеия отсутствуют. Сохранение данных не требуется")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                var iMapper = (new MapperConfiguration(cfg => {
                    cfg.CreateMap<EditViewModel, ShopRole>();
                })).CreateMapper();

                iMapper.Map(model, role);

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    _logger.Information("Role {@Role} edited successfully.", role);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Изменения успехно сохранены.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error edit {RoleName} role.", model.Name);
                return View();
            }
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            return View(await GetEditViewModel(id));
        }

        // POST: Roles/Delete/5
        [Authorize(Roles = RoleNameConstants.RoleManagmentDelete)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            try
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    throw new ApplicationException(UnableLoadRole(id));
                }

                IdentityResult result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    _logger.Information("Role {@Role} deleted successfully.", role);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Роль успешно удалена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error deleting role {RoleID}.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При удалении роли произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helpers
        private async Task<EditViewModel> GetEditViewModel(string id)
        {
            var role = _roleManager.Roles.Where(r => r.Id.ToString() == id).Include(m => m.Module).FirstOrDefault();
            if (role == null)
            {
                throw new ApplicationException(UnableLoadRole(id));
            }

            var modules = new SelectList(await _shopDB.Modules.ToListAsync(), "Id", "Name");

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShopRole, EditViewModel>();
            })).CreateMapper();

            var model = iMapper.Map<ShopRole, EditViewModel>(role);
            model.Modules = modules;

            return model;
        }

        private string UnableLoadRole(string roleID) { return $"Не удалось загрузить роль с идентификатором '{roleID}'."; }

        #endregion
    }

}
