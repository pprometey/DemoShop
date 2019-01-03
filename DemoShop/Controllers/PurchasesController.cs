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
using DemoShop.UI.Models.PurchasesViewModel;
using AutoMapper;

namespace DemoShop.UI.Controllers
{
    [Authorize(Roles = RoleNameConstants.PurchaseInvoiceManagment)]
    [Route("[controller]/[action]")]
    public class PurchasesController : ShopModuleController
    {
        private readonly ILogger _logger;
        private readonly IPurchasesDbRepositories _repository;
        private readonly IGoodsDbRepositories _goodsRepository;

        public PurchasesController(IPurchasesDbRepositories repository, IGoodsDbRepositories goodsRepository)
        {
            _repository = repository;
            _goodsRepository = goodsRepository;
            _logger = Log.ForContext<PurchasesController>();
        }

        public async Task<JsonResult> GetData([FromBody]DataManager dm)
        {
            try
            {
                IEnumerable<PurchaseInvoice> data = await _repository.PurchaseInvoices.GetAllAsync();
                DataOperations operation = new DataOperations();

                if (dm.Sorted != null && dm.Sorted.Count > 0)
                {
                    data = (IEnumerable<PurchaseInvoice>)operation.PerformSorting(data, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    data = (IEnumerable<PurchaseInvoice>)operation.PerformWhereFilter(data, dm.Where, dm.Where[0].Operator);
                }
                int count = data.Cast<PurchaseInvoice>().Count();
                if (dm.Skip != 0)
                {
                    data = (IEnumerable<PurchaseInvoice>)operation.PerformSkip(data, dm.Skip);
                }
                if (dm.Take != 0)
                {
                    data = (IEnumerable<PurchaseInvoice>)operation.PerformTake(data, dm.Take);
                }
                return Json(new { result = data, count = count });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting list of purchases invoce.");
                throw new ApplicationException("Ошибка получения списка покупок");
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

        public async Task<IEnumerable<PurchaseProductViewModel>> GetProducts()
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
                    cfg.CreateMap<Product, PurchaseProductViewModel>()
                        .ForMember("CategoryName", opt => opt.MapFrom(c => c.ProductsCategory.Name))
                        .ForMember("ProductName", opt => opt.MapFrom(src => src.Name + " (" + src.Unit.ShortName + ")")); 
                }
                )).CreateMapper();
            var result = iMapper.Map<IEnumerable<Product>, IEnumerable<PurchaseProductViewModel>>(data);
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

                var iMapper = (new MapperConfiguration(cfg => { cfg.CreateMap<CreateViewModel, PurchaseInvoice>(); })).CreateMapper();
                var item = iMapper.Map<CreateViewModel, PurchaseInvoice>(model);

                var result = await _repository.PurchaseInvoices.CreateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Purchase invoice {@PurchaseInvoice} successfully added.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Покупка успешно добавлена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.Error("Error {@Error} adding purchase invoce {@PurchaseInvoice}.", result.Errors, item);
                }

                AddErrors(result);
                return await PrepareCreateViewModel(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "There was an error adding {@PurchaseInvoice} purchases invoice", model);
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
                var item = await _repository.PurchaseInvoices.GetAsync(model.ID);
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
                    cfg.CreateMap<EditViewModel, PurchaseInvoice>();
                })).CreateMapper();

                iMapper.Map(model, item);

                var result = await _repository.PurchaseInvoices.UpdateAsync(item);
                if (result.Succeeded)
                {
                    _logger.Information("Purchase invoice {@PurchaseInvoice} edited successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Изменения успехно сохранены.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
                return await PrepareEditViewModel(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error edit {PurchaseInvoiceID} purchases invoice.", model.ID);
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

                var result = await _repository.PurchaseInvoices.DeleteAsync(item.ID);
                if (result.Succeeded)
                {
                    _logger.Information("Purchase invoice {@PurchaseInvoice} deleted successfully.", item);
                    StatusMessage = (new AlertMessage(AlertStatus.success, "Покупка успешно удалена.")).SerializeObject();
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error deleting purchase invoice {PurchaseInvoiceID}.", id);
                StatusMessage = (new AlertMessage(AlertStatus.error, "При удалении покупки произошла ошибка.")).SerializeObject();
                return RedirectToAction(nameof(Index));
            }
        }

        #region Helpers

        private async Task<EditViewModel> GetEditViewModel(string id)
        {
            var item = await GetItem(id);

            var iMapper = (new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PurchaseInvoice, EditViewModel>();
            })).CreateMapper();

            var model = iMapper.Map<PurchaseInvoice, EditViewModel>(item);
            model.Products = await GetProducts();
            return model;
        }

        private async Task<PurchaseInvoice> GetItem(string id)
        {
            Guid _id;
            if (String.IsNullOrEmpty(id) || !Guid.TryParse(id, out _id))
            {
                throw new ArgumentException(nameof(id));
            }
            var item = await _repository.PurchaseInvoices.GetAsync(_id);
            if (item == null)
            {
                throw new ApplicationException(UnableLoadItem(id));
            }

            return item;
        }

        private string UnableLoadItem(string id) { return $"Не удалось загрузить покупку с идентификатором '{id}'."; }

        #endregion

    }

}