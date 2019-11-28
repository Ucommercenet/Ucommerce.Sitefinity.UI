using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.Services;

namespace UCommerce.Sitefinity.UI.Mvc.Infrastructure
{
    public class UCommerceMvcTemplateEditorRouteHandler: MvcTemplateEditorRouteHandler
    {
        protected override void ApplyLayoutsAndControls(Page page, RequestContext requestContext)
        {
            base.ApplyLayoutsAndControls(page, requestContext);

            var ucBackendCss = "uc-backend-css";

            if (SystemManager.IsDesignMode && page.Header.FindControl(ucBackendCss) == null)
            {
                var ucommerceCssLink = new HtmlLink
                {
                    ID = ucBackendCss,
                    Href = page.ClientScript.GetWebResourceUrl(typeof(UCommerceMvcPageEditorRouteHandler), "UCommerce.Sitefinity.UI.assets.dist.css.ucommerce-backend.css")
                };

                ucommerceCssLink.Attributes.Add("type", "text/css");
                ucommerceCssLink.Attributes.Add("rel", "stylesheet");

                page.Header.Controls.Add(ucommerceCssLink);
            }
        }
    }
}
