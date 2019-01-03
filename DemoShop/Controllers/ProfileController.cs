using AutoMapper;
using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using DemoShop.UI.Models.Identity.ProfileViewModels;
using DemoShop.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DemoShop.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ProfileController : ShopIdentityController
    {
        private readonly UserManager<ShopUser> _userManager;
        private readonly SignInManager<ShopUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;

        public ProfileController(
          UserManager<ShopUser> userManager,
          SignInManager<ShopUser> signInManager,
          IEmailSender emailSender,
          UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = Log.ForContext<ProfileController>();
            _urlEncoder = urlEncoder;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(_userManager.GetUserId(User)));
            }

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ShopUser, IndexViewModel>();
            })).CreateMapper();
            var model = iMapper.Map<ShopUser, IndexViewModel>(user);
            model.StatusMessage = AlertMessage.DeserializeObject(StatusMessage);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(_userManager.GetUserId(User)));
            }

            bool isChange = false;

            var email = user.Email;
            if (model.Email != email)
            {
                isChange = true;

                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                _logger.Information("Changed email from {OldEmail} to {NewEmail} for {User}.", email, model.Email, user.UserName);

                if (!setEmailResult.Succeeded)
                {
                    _logger.Information("Error {@Error} updating email address {NewEmail} for {User}.", setEmailResult.Errors, model.Email, user.UserName);

                    throw new ApplicationException($"Произошла непредвиденная ошибка при обновлении электронного адреса для пользователя с идентификатором '{user.Id}'.");
                }
                await SendVerificationEmail(model);
            }

            var fullName = user.FullName;
            if (model.FullName != fullName)
            {
                isChange = true;
                user.FullName = model.FullName;
                var setFullNameResult = await _userManager.UpdateAsync(user);
                _logger.Information("Changed full name  from {OldName} to {NewName} for {User}.", fullName, model.FullName, user.UserName);

                if (!setFullNameResult.Succeeded)
                {
                    _logger.Information("Error {@Error} updating full name {NewName} for {User}.", setFullNameResult.Errors, model.FullName, user.UserName);

                    throw new ApplicationException($"Произошла непредвиденная ошибка при обновлении ФИО для пользователя с идентификатором '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                isChange = true;
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                _logger.Information("Changed phone from {OldPhone} to {NewPhone} for {User}.", phoneNumber, model.PhoneNumber, user.UserName);

                if (!setPhoneResult.Succeeded)
                {
                    _logger.Information("Error {@Error} updating phone {NewPhone} for {User}.", setPhoneResult.Errors, model.PhoneNumber, user.UserName);
                    throw new ApplicationException($"Произошла непредвиденная ошибка при обновлении телефона для пользователя с идентификатором  '{user.Id}'.");
                }
            }

            if (isChange) { StatusMessage = (new AlertMessage(AlertStatus.success, "Ваш профиль успешно обновлен.")).SerializeObject(); }
            else { StatusMessage = (new AlertMessage(AlertStatus.info, "Изменеия отсутствуют. Сохранение данных не требуется.")).SerializeObject(); }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(_userManager.GetUserId(User)));
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), code, Request.Scheme);
            await _emailSender.SendEmailConfirmationAsync(user.Email, user.FullName, callbackUrl);
            _logger.Information("Send email with confirmation email {Email} from profile for {User}.", user.Email, user.UserName);

            StatusMessage = (new AlertMessage(AlertStatus.info, "Отправлено электронное письмо с ссылкой для подтверждения электронного адреса. Пожалуйста, проверьте свой адрес электронной почты.")).SerializeObject();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(_userManager.GetUserId(User)));
            }

            var model = new ChangePasswordViewModel { StatusMessage = AlertMessage.DeserializeObject(StatusMessage) };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (model == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(UnableLoadUser(_userManager.GetUserId(User)));
            }

            if (await _userManager.CheckPasswordAsync(user, model.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "Новый пароль не должен совпадать с действующим");
                return View(model);
            };

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.Information("User changed their password successfully for {User}.", user.UserName);
            StatusMessage = (new AlertMessage(AlertStatus.success, "Пароль успешно изменен.")).SerializeObject();
            return RedirectToAction(nameof(ChangePassword));
        }
    }
}