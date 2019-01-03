using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DemoShop.Core.Constants;
using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using DemoShop.UI.Models.Goods.ProductsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataSources;

namespace DemoShop.UI.Controllers
{
    [Authorize(Roles = RoleNameConstants.ProductManagment)]
    [Route("[controller]/[action]")]
    public class ProductsController : ShopModuleController
    {
        private readonly ILogger _logger;
        private readonly IGoodsDbRepositories _repository;

        public ProductsController(
            IGoodsDbRepositories repository)
        {
            _repository = repository;
            _logger = Log.ForContext<ProductsController>();
        }

        public async Task<JsonResult> GetData([FromBody]DataManager dm)
        {
            try
            {
                IEnumerable<Product> data = await _repository.Products.GetAllAsync();
                DataOperations operation = new DataOperations();

                if (dm.Sorted != null && dm.Sorted.Count > 0)
                {
                    data = (IEnumerable<Product>)operation.PerformSorting(data, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    data = (IEnumerable<Product>)operation.PerformWhereFilter(data, dm.Where, dm.Where[0].Operator);
                }
                int count = data.Cast<Product>().Count();
                if (dm.Skip != 0)
                {
                    data = (IEnumerable<Product>)operation.PerformSkip(data, dm.Skip);
                }
                if (dm.Take != 0)
                {
                    data = (IEnumerable<Product>)operation.PerformTake(data, dm.Take);
                }
                return Json(new { result = data, count = count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of products.");
                throw new ApplicationException("Ошибка получения списка товаров");
            }
        }

        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                StatusMessage = AlertMessage.DeserializeObject(StatusMessage)
            };
            return View(model);
        }

        // GET: BusinessAgents/Details/5
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

        // GET: BusinessAgents/Create
        public async Task<IActionResult> Create()
        {
            return await PrepareCreateViewModel(new CreateViewModel());
        }

        private async Task<IActionResult> PrepareCreateViewModel(CreateViewModel model)
        {
            model.ProductsCategories = await GetProductsCategories();
            model.Units = await GetUnits();
            return View(model);
        }

        public async Task<IEnumerable<ProductProductsCategoryViewModel>> GetProductsCategories()
        {
            IEnumerable<ProductsCategory> data = null;
            try
            {
                data = await _repository.ProductsCategories.GetAllAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of products category.");
                throw new ApplicationException("Ошибка получения списка категорий продуктов");
            }

            var iMapper = (new MapperConfiguration(cfg => { cfg.CreateMap<ProductsCategory, ProductProductsCategoryViewModel>(); })).CreateMapper();
            var result = iMapper.Map<IEnumerable<ProductsCategory>, IEnumerable<ProductProductsCategoryViewModel>>(data);
            return result;
        }

        public async Task<IEnumerable<ProductUnitViewModel>> GetUnits()
        {
            IEnumerable<Unit> data = null;
            try
            {
                data = await _repository.Units.GetAllAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of units.");
                throw new ApplicationException("Ошибка получения списка единиц измерения");
            }

            var iMapper = (new MapperConfiguration(cfg => { cfg.CreateMap<Unit, ProductUnitViewModel>(); })).CreateMapper();
            var result = iMapper.Map<IEnumerable<Unit>, IEnumerable<ProductUnitViewModel>>(data);
            return result;
        }

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
                    return await PrepareCreateViewModel(model);
                }

                var iMapper = (new MapperConfiguration(cfg => { cfg.CreateMap<CreateViewModel, Product>(); })).CreateMapper();
                var item = iMapper.Map<CreateViewModel, Product>(model);

                var result = await _repository.Products.CreateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Product {@Product} successfully added.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Товар успешно добавлен.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.Error("Error {@Error} adding product {@Product}.", result.Errors, item);
                }

                AddErrors(result);
                return await PrepareCreateViewModel(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There was an error adding {ProductName} organization", model.Name);
                return View();
            }
        }

        // GET: BusinessAgents/Edit/5
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


        private async Task<IActionResult> PrepareEditViewModel(EditViewModel model)
        {
            model.ProductsCategories = await GetProductsCategories();
            model.Units = await GetUnits();
            return View(model);
        }

        // POST: BusinessAgents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                return await PrepareEditViewModel(model);
            }
            try
            {
                var item = await _repository.Products.GetAsync(model.ID);
                if (item == null)
                {
                    throw new ApplicationException(UnableLoadItem(model.ID.ToString()));
                }

                if (!ModelState.IsValid)
                {
                    return await PrepareEditViewModel(model);
                }


                if (!model.IsModifed(item))
                {
                    StatusMessage = (new AlertMessage(AlertStatus.info, "Изменеия отсутствуют. Сохранение данных не требуется")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                var iMapper = (new MapperConfiguration(cfg => {
                    cfg.CreateMap<EditViewModel, Product>();
                })).CreateMapper();

                iMapper.Map(model, item);

                var result = await _repository.Products.UpdateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Product {@Product} edited successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Изменения успехно сохранены.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
                return await PrepareEditViewModel(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error edit {ProductID} product.", model.ID);
                return View();
            }

        }

        // GET: BusinessAgents/Delete/5
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


        // POST: BusinessAgents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var item = await GetItem(id);

                var result = await _repository.Products.DeleteAsync(item.ID);
                if (result.Succeeded)
                {
                    _logger.Information("Product {@Product} deleted successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Товар успешно удален.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error deleting product {ProductID}.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При удалении товара произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helpers

        private async Task<EditViewModel> GetEditViewModel(string id)
        {
            var item = await GetItem(id);

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, EditViewModel>();
            })).CreateMapper();

            var model = iMapper.Map<Product, EditViewModel>(item);
            model.ProductsCategories = await GetProductsCategories();
            model.Units = await GetUnits();
            return model;
        }

        private async Task<Product> GetItem(string id)
        {
            Guid _id;
            if (String.IsNullOrEmpty(id) || !Guid.TryParse(id, out _id))
            {
                throw new ArgumentException(nameof(id));
            }
            var item = await _repository.Products.GetAsync(_id);
            if (item == null)
            {
                throw new ApplicationException(UnableLoadItem(id));
            }

            return item;
        }

        private string UnableLoadItem(string id) { return $"Не удалось загрузить товар с идентификатором '{id}'."; }

        #endregion

    }

}