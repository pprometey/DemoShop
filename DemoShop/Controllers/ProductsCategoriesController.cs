using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DemoShop.UI.Models.Goods.ProductsCategoriesViewModels;
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

    [Authorize(Roles = RoleNameConstants.ProductsCategoryManagment)]
    [Route("[controller]/[action]")]

    public class ProductsCategoriesController : ShopModuleController
    {
        private readonly ILogger _logger;
        private readonly IGoodsDbRepositories _repository;

        public ProductsCategoriesController(
            IGoodsDbRepositories repository)
        {
            _repository = repository;
            _logger = Log.ForContext<ProductsCategoriesController>();
        }

        public async Task<JsonResult> GetData([FromBody]DataManager dm)
        {
            try
            {
                IEnumerable<ProductsCategory> data = await _repository.ProductsCategories.GetAllAsync();
                DataOperations operation = new DataOperations();

                if (dm.Sorted != null && dm.Sorted.Count > 0)
                {
                    data = (IEnumerable<ProductsCategory>)operation.PerformSorting(data, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    data = (IEnumerable<ProductsCategory>)operation.PerformWhereFilter(data, dm.Where, dm.Where[0].Operator);
                }
                int count = data.Cast<ProductsCategory>().Count();
                if (dm.Skip != 0)
                {
                    data = (IEnumerable<ProductsCategory>)operation.PerformSkip(data, dm.Skip);
                }
                if (dm.Take != 0)
                {
                    data = (IEnumerable<ProductsCategory>)operation.PerformTake(data, dm.Take);
                }
                return Json(new { result = data, count = count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of products categories.");
                throw new ApplicationException("Ошибка получения списка категорий товаров");
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

                var item = new ProductsCategory { Name = model.Name };

                var result = await _repository.ProductsCategories.CreateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Products category {@ProductsCategory} successfully added.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Категория продукта успешно добавлена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.Error("Error {@Error} adding products category {@ProductsCategory}.", result.Errors, item);
                }

                AddErrors(result);
                return View(model);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "There was an error adding {ProductsCategorytName} products category", model.Name);
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
                var item = await _repository.ProductsCategories.GetAsync(model.ID);
                if (item == null)
                {
                    throw new ApplicationException(UnableLoadItem(model.ID.ToString()));
                }

                if (model.Name == item.Name)
                {
                    StatusMessage = (new AlertMessage(AlertStatus.info, "Изменеия отсутствуют. Сохранение данных не требуется")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                var iMapper = (new MapperConfiguration(cfg => {
                    cfg.CreateMap<EditViewModel, ProductsCategory>();
                })).CreateMapper();

                iMapper.Map(model, item);

                var result = await _repository.ProductsCategories.UpdateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Products category {@ProductsCategory} edited successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Изменения успехно сохранены.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error edit {ProductsCategoryId} products category.", model.ID);
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

                var result = await _repository.ProductsCategories.DeleteAsync(item.ID);
                if (result.Succeeded)
                {
                    _logger.Information("Products category {@ProductsCategory} deleted successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Категория продукта успешно удалена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error deleting products category {ProductsCategoryId}.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При удалении категории продукта произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helpers
        private async Task<EditViewModel> GetEditViewModel(string id)
        {

            var item = await GetItem(id);

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductsCategory, EditViewModel>();
            })).CreateMapper();

            var model = iMapper.Map<ProductsCategory, EditViewModel>(item);

            return model;
        }

        private async Task<ProductsCategory> GetItem(string id)
        {
            Guid _id;
            if (String.IsNullOrEmpty(id) || !Guid.TryParse(id, out _id))
            {
                throw new ArgumentException(nameof(id));
            }
            var item = await _repository.ProductsCategories.GetAsync(_id);
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
