using AutoMapper;
using DemoShop.Core.Constants;
using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using DemoShop.UI.Models.Identity.UsersViewModels;
using DemoShop.UI.Services;
using DemoShop.UI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles = RoleNameConstants.UserManagement)]
    [Route("[controller]/[action]")]
    public class UsersController : ShopIdentityController
    {
        private readonly UserManager<ShopUser> _userManager;
        private readonly RoleManager<ShopRole> _roleManager;
        private readonly SignInManager<ShopUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;

        public UsersController(
            UserManager<ShopUser> userManager,
            RoleManager<ShopRole> roleManager,
            SignInManager<ShopUser> signInManager,
            IEmailSender emailSender)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = Log.ForContext<UsersController>();
            _emailSender = emailSender;
        }

        public async Task<JsonResult> GetData([FromBody]DataManager dm)
        {
            try
            {
                IEnumerable<ShopUser> data = await _userManager.Users.AsNoTracking().ToListAsync();
                DataOperations operation = new DataOperations();

                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    data = (IEnumerable<ShopUser>)operation.PerformSorting(data, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    data = (IEnumerable<ShopUser>)operation.PerformWhereFilter(data, dm.Where, dm.Where[0].Operator);
                }
                int count = data.Cast<ShopUser>().Count();
                if (dm.Skip != 0)
                {
                    data = (IEnumerable<ShopUser>)operation.PerformSkip(data, dm.Skip);
                }
                if (dm.Take != 0)
                {
                    data = (IEnumerable<ShopUser>)operation.PerformTake(data, dm.Take);
                }
                return Json(new { result = data, count = count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error get users.");
                throw new ApplicationException("Error get users");
            }

        }

        // GET: Users
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                StatusMessage = AlertMessage.DeserializeObject(StatusMessage)
            };
            return View(model);
        }


        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            var model = new CreateViewModel
            {
                Roles = await GetRolesTree()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model, List<string> newRoles)
        {
            if (model == null)
            {
                return NotFound();
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Roles = await GetRolesTree(newRoles);
                    return View(model);
                }

                var iMapper = (new MapperConfiguration(cfg => {
                    cfg.CreateMap<CreateViewModel, ShopUser>();
                })).CreateMapper();
                var user = iMapper.Map<CreateViewModel, ShopUser>(model);
                user.UserName = user.Email;

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.Information("User {@User} successfully added.", user);

                    await SendVerificationEmail(user);

                    var resultRole = await _userManager.AddToRolesAsync(user, newRoles);
                    if (resultRole.Succeeded)
                    {
                        _logger.Information("User {UserName} roles {@Roles} successfully added", user.UserName, newRoles);
                        StatusMessage = (new AlertMessage(AlertStatus.success, "Пользователь успешно добавлен!")).SerializeObject();
                        return RedirectToAction(nameof(Index));
                    }
                    AddErrors(resultRole);
                }
                AddErrors(result);

                model.Roles = await GetRolesTree(newRoles);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error adding {UserName} user.", model.Email);
                return View();
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            return View(await GetEditViewModel(id));
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model, List<string> newRoles)
        {
            if (model == null)
            {
                return NotFound();
            }
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user == null)
                {
                    throw new ApplicationException(UnableLoadUser(model.Id.ToString()));
                }

                if (!ModelState.IsValid)
                {
                    model.UserRoles = await GetUserRoles(user);
                    model.Roles = await GetRolesTree(newRoles);
                    return View(model);
                }

                if (model.FullName == user.FullName && model.PhoneNumber == user.PhoneNumber && model.Email == user.Email && model.UserRoles.Equals(newRoles))
                {
                    StatusMessage = (new AlertMessage(AlertStatus.info, "Изменеия отсутствуют. Сохранение данных не требуется")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                bool isChangeEmail = user.Email == model.Email ? false : true;

                var iMapper = (new MapperConfiguration(cfg => {
                    cfg.CreateMap<EditViewModel, ShopUser>();
                })).CreateMapper();

                iMapper.Map(model, user);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.Information("User {@User} edited successfully.", user);

                    if (isChangeEmail == true)
                    {
                        await SendVerificationEmail(user);
                    };

                    var userRoles = await GetUserRoles(user);
                    var allRoles = _roleManager.Roles.ToList();
                    var addedRoles = newRoles.Except(userRoles);
                    var removedRoles = userRoles.Except(newRoles);

                    var resultAddRole = await _userManager.AddToRolesAsync(user, addedRoles);
                    if (resultAddRole.Succeeded)
                    {
                        _logger.Information("When editing user {UserName}, roles {@AddedRoles} were successfully added.", user.UserName, addedRoles);
                        var resultRemoveRole = await _userManager.RemoveFromRolesAsync(user, removedRoles);
                        if (resultRemoveRole.Succeeded)
                        {
                            _logger.Information("When editing user {UserName}, roles {@DeletedRoles} were successfully deleted.", user.UserName, removedRoles);

                            StatusMessage = (new AlertMessage(AlertStatus.success, "Изменения успешно сохранены!")).SerializeObject();
                            if (user.UserName != User.Identity.Name)
                            {
                                return RedirectToAction(nameof(Index));
                            }
                            else return new LogoutResult(_signInManager);
                        }
                        else
                        {
                            _logger.Error("When editing user {UserName}, an error occurred while deleting roles {@DeletedRoles}.", user.UserName, removedRoles);
                        }
                        AddErrors(resultRemoveRole);
                    }
                    else
                    {
                        _logger.Error("When editing user {UserName}, an error occurred while adding roles {@DeletedRoles}.", user.UserName, addedRoles);
                    }
                    AddErrors(resultAddRole);
                }
                else
                {
                    _logger.Error("When editing user {@User}, an error occurred", user);

                }
                AddErrors(result);

                model.UserRoles = await GetUserRoles(user);
                model.Roles = await GetRolesTree(newRoles);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error editing user {UserName}.", model.UserName);
                return View();
            }
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            return View(await GetEditViewModel(id));
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            return View(await GetEditViewModel(id));
        }

        // POST: Users/Delete/5
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
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new ApplicationException(UnableLoadUser(id));
                }

                if (user.UserName == User.Identity.Name)
                {
                    StatusMessage = (new AlertMessage(AlertStatus.warning, "Нельзя удалить самого себя.")).SerializeObject();
                    _logger.Warning("User {@User} tries to delete himself.", user);
                    return RedirectToAction(nameof(Index));

                }

                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.Information("User {@User} deleted successfully.", user);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Пользователь успешно удален.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.Error("When deleting user {@User}, an error occurred.", user);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error deleting user {UserID}.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При  удалении пользователя произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }

        }

        public async Task<IActionResult> ResetPassword(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(id));
            }
            ResetPasswordViewModel model = new ResetPasswordViewModel { Id = user.Id, FullName = user.FullName };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
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
                var id = model.Id.ToString();

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new ApplicationException(UnableLoadUser(id));
                }
                var _passwordValidator =
                    HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ShopUser>)) as IPasswordValidator<ShopUser>;
                var _passwordHasher =
                    HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ShopUser>)) as IPasswordHasher<ShopUser>;

                var result =
                    await _passwordValidator.ValidateAsync(_userManager, user, model.Password);
                if (result.Succeeded)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

                    var resultUpdate = await _userManager.UpdateAsync(user);
                    if (resultUpdate.Succeeded)
                    {
                        _logger.Information("User {UserName} password reset.", user.UserName);
                        StatusMessage = (new AlertMessage(AlertStatus.success, "Пароль успешно изменен.")).SerializeObject();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.Error("When you reset the password for {UserName}, an error {@Errors} occurred", user.UserName, resultUpdate.Errors);
                    }
                    AddErrors(resultUpdate);
                }
                else
                {
                    _logger.Error("When you reset the password for {UserName}, password authentication error {@Error} occurred", user.UserName, result.Errors);
                }
                AddErrors(result);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while resetting the password for user {UserID}.", model.Id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При сбросе пароля произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }


        public async Task<IActionResult> EditRights(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(id));
            }

            var uRoles = await GetUserRoles(user);
            EditRightsViewModel model = new EditRightsViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserRoles = uRoles,
                Roles = await GetRolesTree(uRoles)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRights(string id, List<string> newRoles)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(id));
            }

            try
            {
                var userRoles = await GetUserRoles(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = newRoles.Except(userRoles);
                var removedRoles = userRoles.Except(newRoles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                _logger.Information("Permissions for {UserName} changed successfully. {@AddedRoles} {@RemovedRoles}", user.UserName, addedRoles, removedRoles);
                StatusMessage = (new AlertMessage(AlertStatus.success, "Роли пользователя успешно изменены.")).SerializeObject();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while changing the user's {UserID} rights.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При изменении прав пользователя произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailConfirmationAsync(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(id));
            }

            try
            {
                await SendVerificationEmail(user);
                StatusMessage = (new AlertMessage(AlertStatus.success, "Письмо для проверки электронного адреса успешно отправлено.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "When sending a message to the user {UserID} with the confirmation of the email address, an error occurred.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При отправке письма для подтверждения электронного адреса произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helpers

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SendVerificationEmail(ShopUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, user.FullName, callbackUrl);
            _logger.Information("The email was successfully sent to the user {UserName} to confirm the email address.", user.UserName);

        }

        private async Task<EditViewModel> GetEditViewModel(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(id));
            }

            var iMapper = (new MapperConfiguration(cfg => {
                cfg.CreateMap<ShopUser, EditViewModel>();
            })).CreateMapper();
            var model = iMapper.Map<ShopUser, EditViewModel>(user);

            model.UserRoles = await GetUserRoles(user);
            model.Roles = await GetRolesTree(model.UserRoles);

            return model;
        }

        private async Task<IEnumerable<RolesTreeView>> GetRolesTree(IEnumerable<string> userRoles = null)
        {
            List<RolesTreeView> result = new List<RolesTreeView>();
            if (userRoles == null) userRoles = new List<string>();

            var allRoles = await _roleManager.Roles.Include(m => m.Module).ToListAsync();
            var allModules = allRoles.Select(m => m.Module).Distinct().OrderBy(o => o.Name);

            foreach (ShopModule module in allModules)
            {
                var moduleItem = new RolesTreeView
                {
                    Id = module.Id,
                    Text = module.Name,
                    hasChild = true,
                    expanded = true
                };
                result.Add(moduleItem);

                var moduleRoles = allRoles.Where(t => t.ModuleID == module.Id).OrderBy(p => p.Name);
                foreach (ShopRole role in moduleRoles)
                {
                    var roleItem = new RolesTreeView
                    {
                        Id = role.Id,
                        ParentId = module.Id,
                        Name = role.Name,
                        Text = role.Description,
                        hasChild = false,
                        isChecked = userRoles.Contains(role.Name) ? true : false
                    };
                    result.Add(roleItem);
                }
            }
            return result;
        }

        private async Task<IEnumerable<string>> GetUserRoles(ShopUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            return await _userManager.GetRolesAsync(user);
        }

        #endregion
    }

}
