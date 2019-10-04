using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;
using Ucommerce.Sitefinity.UI.Mvc.Model;
using UCommerce.EntitiesV2;

namespace Ucommerce.Sitefinity.UI.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "uConfirmationEmail_MVC", Title = "Confirmation Email", SectionName = UcommerceUIModule.UCOMMERCE_WIDGET_SECTION, ModuleName = UcommerceUIModule.NAME, CssClass = "sfMvcIcn")]
    public class ConfirmationEmailController : Controller
    {
        public ActionResult Index()
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();

            var orderGuid = System.Web.HttpContext.Current.Request.QueryString["orderGuid"];
            var model = ResolveModel();
            var confirmationEmailVM = model.GetViewModel(orderGuid);

            ViewBag.RowSpan = 4;
            if (purchaseOrder.DiscountTotal > 0)
            {
                ViewBag.RowSpan++;
            }
            if (purchaseOrder.ShippingTotal > 0)
            {
                ViewBag.RowSpan++;
            }
            if (purchaseOrder.PaymentTotal > 0)
            {
                ViewBag.RowSpan++;
            }

            return View("Index", confirmationEmailVM);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.ActionInvoker.InvokeAction(this.ControllerContext, "Index");
        }

        public ConfirmationEmailModel ResolveModel()
        {
            var model = new ConfirmationEmailModel();

            return model;
        }
    }
}
