using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoShop.Core.Constants;
using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataSources;
using DemoShop.UI.Models.SalesViewModel;
using AutoMapper;

namespace DemoShop.UI.Controllers
{
    [Authorize(Roles = RoleNameConstants.SalesInvoiceManagment)]
    [Route("[controller]/[action]")]
    public class SalesController : ShopModuleController
    {
        private readonly ILogger _logger;
        private readonly ISalesDbRepositories _repository;
        private readonly IGoodsDbRepositories _goodsRepository;

        public SalesController(ISalesDbRepositories repository, IGoodsDbRepositories goodsRepository)
        {
            _repository = repository;
            _goodsRepository = goodsRepository;
            _logger = Log.ForContext<PurchasesController>();
        }

        public async Task<JsonResult> GetData([FromBody]DataManager dm)
        {
            try
            {
                IEnumerable<SalesInvoice> data = await _repository.SalesInvoices.GetAllAsync();
                DataOperations operation = new DataOperations();

                if (dm.Sorted != null && dm.Sorted.Count > 0)
                {
                    data = (IEnumerable<SalesInvoice>)operation.PerformSorting(data, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    data = (IEnumerable<SalesInvoice>)operation.PerformWhereFilter(data, dm.Where, dm.Where[0].Operator);
                }
                int count = data.Cast<SalesInvoice>().Count();
                if (dm.Skip != 0)
                {
                    data = (IEnumerable<SalesInvoice>)operation.PerformSkip(data, dm.Skip);
                }
                if (dm.Take != 0)
                {
                    data = (IEnumerable<SalesInvoice>)operation.PerformTake(data, dm.Take);
                }
                return Json(new { result = data, count = count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of sales invoce.");
                throw new ApplicationException("Ошибка получения списка продаж");
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
            model.Products = await GetProducts();
            return View(model);
        }

        public async Task<IEnumerable<SalesProductViewModel>> GetProducts()
        {
            IEnumerable<Product> data = null;
            try
            {
                data = await _goodsRepository.Products.GetAllAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of products.");
                throw new ApplicationException("Ошибка получения списка товаров");
            }

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, SalesProductViewModel>()
                    .ForMember("CategoryName", opt => opt.MapFrom(c => c.ProductsCategory.Name))
                    .ForMember("ProductName", opt => opt.MapFrom(src => src.Name + " (" + src.Unit.ShortName + ")"));
            }
                )).CreateMapper();
            var result = iMapper.Map<IEnumerable<Product>, IEnumerable<SalesProductViewModel>>(data);
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

                var iMapper = (new MapperConfiguration(cfg => { cfg.CreateMap<CreateViewModel, SalesInvoice>(); })).CreateMapper();
                var item = iMapper.Map<CreateViewModel, SalesInvoice>(model);

                var result = await _repository.SalesInvoices.CreateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Sales invoice {@SalesInvoice} successfully added.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Продажа успешно добавлена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.Error("Error {@Error} adding sales invoce {@SalesInvoice}.", result.Errors, item);
                }

                AddErrors(result);
                return await PrepareCreateViewModel(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There was an error adding {@SalesInvoice} sales invoice", model);
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
            model.Products = await GetProducts();
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
                var item = await _repository.SalesInvoices.GetAsync(model.ID);
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
                    cfg.CreateMap<EditViewModel, SalesInvoice>();
                })).CreateMapper();

                iMapper.Map(model, item);

                var result = await _repository.SalesInvoices.UpdateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Sales invoice {@SalesInvoice} edited successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Изменения успехно сохранены.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
                return await PrepareEditViewModel(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error edit {SalesInvoiceID} sales invoice.", model.ID);
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

                var result = await _repository.SalesInvoices.DeleteAsync(item.ID);
                if (result.Succeeded)
                {
                    _logger.Information("Sales invoice {@SalesInvoice} deleted successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Покупка успешно удалена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error deleting sales invoice {SalesInvoiceID}.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При удалении продажи произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helpers

        private async Task<EditViewModel> GetEditViewModel(string id)
        {
            var item = await GetItem(id);

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SalesInvoice, EditViewModel>();
            })).CreateMapper();

            var model = iMapper.Map<SalesInvoice, EditViewModel>(item);
            model.Products = await GetProducts();
            return model;
        }

        private async Task<SalesInvoice> GetItem(string id)
        {
            Guid _id;
            if (String.IsNullOrEmpty(id) || !Guid.TryParse(id, out _id))
            {
                throw new ArgumentException(nameof(id));
            }
            var item = await _repository.SalesInvoices.GetAsync(_id);
            if (item == null)
            {
                throw new ApplicationException(UnableLoadItem(id));
            }

            return item;
        }

        private string UnableLoadItem(string id) { return $"Не удалось загрузить продажу с идентификатором '{id}'."; }

        #endregion

    }

}