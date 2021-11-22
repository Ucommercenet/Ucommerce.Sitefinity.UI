using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Ucommerce.Api;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
	/// <summary>
	/// The controller class for the Products MVC widget.
	/// </summary>
	[ControllerToolboxItem(Name = "uProducts_MVC", Title = "Products", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnProducts sfMvcIcn")]
	public class ProductsController : Controller, IPersonalizable
	{
		public int ItemsPerPage { get; set; } = 10;

		public bool OpenInSamePage { get; set; } = true;

		public Guid DetailsPageId { get; set; }

		public bool IsManualSelectionMode { get; set; }

		public string ProductIds { get; set; }

		public string CategoryIds { get; set; }

		public bool EnableCategoryFallback { get; set; }

		public string FallbackCategoryIds { get; set; }

		public string ListTemplateName { get; set; } = "Index";

		public string DetailTemplateName { get; set; } = "Index";

		public bool EnableSEOPageTitle { get; set; } = true;

		[OutputCache(Duration = 30, VaryByParam = "*")]
		[RelativeRoute("{categoryName?}")]
		[RelativeRoute("{parentCategory1?}/{categoryName?}")]
		[RelativeRoute("{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
		[RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
		[RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
		[RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName?}")]
		public ActionResult Index()
		{
			ProductListViewModel viewModel;
			try
			{
				var productModel = this.ResolveModel();
				string message;
				var parameters = new System.Collections.Generic.Dictionary<string, object>();

				if (!productModel.CanProcessRequest(parameters, out message))
				{
					return this.PartialView("_Warning", message);
				}

				viewModel = productModel.CreateListViewModel();
				var templateName = listTemplateNamePrefix + this.ListTemplateName;

				return this.View(templateName, viewModel);
			}
			catch (Exception ex)
			{
				if (UCommerceUIModule.TryHandleSystemError(ex, out ActionResult actionResult))
				{
					return actionResult;
				}
				else
				{
					throw;
				}
			}
		}

		[RelativeRoute("{categoryName}/p/{productId}")]
		[RelativeRoute("{parentCategory1?}/{categoryName}/p/{productId}")]
		[RelativeRoute("{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
		[RelativeRoute("{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
		[RelativeRoute("{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
		[RelativeRoute("{parentCategory5?}/{parentCategory4?}/{parentCategory3?}/{parentCategory2?}/{parentCategory1?}/{categoryName}/p/{productId}")]
		public ActionResult Details()
		{

			var productModel = this.ResolveModel();
			string message;
			var parameters = new System.Collections.Generic.Dictionary<string, object>();

			if (!productModel.CanProcessRequest(parameters, out message))
			{
				return this.PartialView("_Warning", message);
			}

			var viewModel = productModel.CreateDetailsViewModel();
			var templateName = this.detailTemplateNamePrefix + this.DetailTemplateName;

			if (this.EnableSEOPageTitle)
			{
				this.ViewBag.Title = viewModel.DisplayName;
			}

			return this.View(templateName, viewModel);
		}

		protected override void HandleUnknownAction(string actionName)
		{
			this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
		}

		private IProductModel ResolveModel()
		{
			var args = new Castle.MicroKernel.Arguments();

			args.AddProperties(new
			{
				itemsPerPage = this.ItemsPerPage,
				openInSamePage = this.OpenInSamePage,
				isManualSelectionMode = this.IsManualSelectionMode,
				enableCategoryFallback = this.EnableCategoryFallback,
				detailsPageId = this.DetailsPageId,
				productIds = this.ProductIds,
				categoryIds = this.CategoryIds,
				fallbackCategoryIds = this.FallbackCategoryIds
			});

			var container = UCommerceUIModule.Container;
			var model = container.Resolve<IProductModel>(args);

			return model;
		}

		private string listTemplateNamePrefix = "List.";
		private string detailTemplateNamePrefix = "Detail.";
	}
}