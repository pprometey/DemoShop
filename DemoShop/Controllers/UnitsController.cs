using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DemoShop.UI.Models.Goods.UnitsViewModels;
using Microsoft.AspNetCore.Authorization;
using DemoShop.Core.Constants;
using DemoShop.Core.Infrastructure;
using Serilog;
using Syncfusion.JavaScript;
using DemoShop.Core.Domain;
using Syncfusion.JavaScript.DataSources;
using AutoMapper;

namespace DemoShop.UI.Controllers
{
    [Authorize(Roles = RoleNameConstants.UnitManagment)]
    [Route("[controller]/[action]")]

    public class UnitsController : ShopModuleController
    {
        private readonly ILogger _logger;
        private readonly IGoodsDbRepositories _repository;

        public UnitsController(
            IGoodsDbRepositories repository)
        {
            _repository = repository;
            _logger = Log.ForContext<UnitsController>();
        }

        public async Task<JsonResult> GetData([FromBody]DataManager dm)
        {
            try
            {
                IEnumerable<Unit> data = await _repository.Units.GetAllAsync();
                DataOperations operation = new DataOperations();

                if (dm.Sorted != null && dm.Sorted.Count > 0)
                {
                    data = (IEnumerable<Unit>)operation.PerformSorting(data, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) 
                {
                    data = (IEnumerable<Unit>)operation.PerformWhereFilter(data, dm.Where, dm.Where[0].Operator);
                }
                int count = data.Cast<Unit>().Count();
                if (dm.Skip != 0)
                {
                    data = (IEnumerable<Unit>)operation.PerformSkip(data, dm.Skip);
                }
                if (dm.Take != 0)
                {
                    data = (IEnumerable<Unit>)operation.PerformTake(data, dm.Take);
                }
                return Json(new { result = data, count = count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of units.");
                throw new ApplicationException("Ошибка получения списка единиц измерения");
            }

        }

        // GET: Posts
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                StatusMessage = AlertMessage.DeserializeObject(StatusMessage)
            };
            return View(model);

        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var viewModel = await GetEditViewModel(id);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);

        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            CreateViewModel model = new CreateViewModel();
            return View(model);
        }

        // POST: Posts/Create
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

                var item = new Unit { Name = model.Name, ShortName = model.ShortName };

                var result = await _repository.Units.CreateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Unit {@Unit} successfully added.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Единица измерения успешно добавлена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.Error("Error {@Error} adding unit {@Unit}.", result.Errors, item);
                }

                AddErrors(result);
                return View(model);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "There was an error adding {UnitShortName} unit", model.ShortName);
                return View();
            }
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var viewModel = await GetEditViewModel(id);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: Posts/Edit/5
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
                var item = await _repository.Units.GetAsync(model.ID);
                if (item == null)
                {
                    throw new ApplicationException(UnableLoadItem(model.ID.ToString()));
                }

                if (model.Name == item.Name && model.ShortName == item.ShortName)
                {
                    StatusMessage = (new AlertMessage(AlertStatus.info, "Изменеия отсутствуют. Сохранение данных не требуется")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                var iMapper = (new MapperConfiguration(cfg => {
                    cfg.CreateMap<EditViewModel, Unit>();
                })).CreateMapper();

                iMapper.Map(model, item);

                var result = await _repository.Units.UpdateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Unit {@Unit} edited successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Изменения успехно сохранены.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error edit {UnitName} unit.", model.Name);
                return View();
            }

        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var viewModel = await GetEditViewModel(id);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var item = await GetItem(id);

                var result = await _repository.Units.DeleteAsync(item.ID);
                if (result.Succeeded)
                {
                    _logger.Information("Unit {@Unit} deleted successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Единица измерения успешно удалена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error deleting unit {UnitID}.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При удалении единицы измерения произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helpers
        private async Task<EditViewModel> GetEditViewModel(string id)
        {

            var item = await GetItem(id);

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Unit, EditViewModel>();
            })).CreateMapper();

            var model = iMapper.Map<Unit, EditViewModel>(item);

            return model;
        }

        private async Task<Unit> GetItem(string id)
        {
            Guid _id;
            if (String.IsNullOrEmpty(id) || !Guid.TryParse(id, out _id))
            {
                throw new ArgumentException(nameof(id));
            }
            var item = await _repository.Units.GetAsync(_id);
            if (item == null)
            {
                throw new ApplicationException(UnableLoadItem(id));
            }

            return item;
        }

        private string UnableLoadItem(string id) { return $"Не удалось загрузить единицу измерения с идентификатором '{id}'."; }

        #endregion
    }
}
