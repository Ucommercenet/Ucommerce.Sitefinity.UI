using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Personalization;
using Telerik.Sitefinity.Services;
using UCommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.Sitefinity.UI.Mvc.ViewModels;
using UCommerce.Infrastructure;
using UCommerce.Transactions;

namespace UCommerce.Sitefinity.UI.Mvc.Controllers
{
    /// <summary>
    /// The controller class for the Basket Preview MVC widget.
    /// </summary>
    [ControllerToolboxItem(Name = "uBasketPreview_MVC", Title = "Basket Preview", SectionName = UCommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UCommerceUIModule.NAME, CssClass = "ucIcnBasketPreview sfMvcIcn")]
    public class BasketPreviewController : Controller, IPersonalizable
    {
        public Guid? NextStepId { get; set; }
        public Guid? PreviousStepId { get; set; }
        public string TemplateName { get; set; } = "Index";

        public ActionResult Index()
        {
            var model = ResolveModel();

            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var basketPreviewViewModel = model.GetViewModel();
            
            ViewBag.RowSpan = 4;
            if (basketPreviewViewModel.DiscountAmount > 0)
            {
                ViewBag.RowSpan++;
            }
            if (basketPreviewViewModel.ShipmentAmount > 0)
            {
                ViewBag.RowSpan++;
            }
            if (basketPreviewViewModel.PaymentAmount > 0)
            {
                ViewBag.RowSpan++;
            }

            var detailTemplateName = this.detailTemplateNamePrefix + this.TemplateName;

            return View(detailTemplateName, basketPreviewViewModel);
        }

        [HttpPost]
        public ActionResult RequestPayment()
        {
            var model = ResolveModel();
            string message;
            var parameters = new System.Collections.Generic.Dictionary<string, object>();

            if (!model.CanProcessRequest(parameters, out message))
            {
                return this.PartialView("_Warning", message);
            }

            var paymentUrl = model.GetPaymentUrl();

            return Redirect(paymentUrl);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            this.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        private IBasketPreviewModel ResolveModel()
        {
            var args = new Castle.MicroKernel.Arguments();

            args.AddProperties(new
            {
                nextStepId = this.NextStepId,
                previousStepId = this.PreviousStepId
            });

            var container = UCommerceUIModule.Container;
            var model = container.Resolve<IBasketPreviewModel>(args);

            return model;
        }

        private string detailTemplateNamePrefix = "Detail.";
    }
}
