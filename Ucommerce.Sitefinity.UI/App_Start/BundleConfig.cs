using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace UCommerce.Sitefinity.UI.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/ucommerce-js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/require-2.3.2.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsAddress.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsAddToBasketButton.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsMiniBasket.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsPaymentPicker.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsQuantityPicker.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsShippingPicker.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsUpdateBasket.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsVariantPicker.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsVoucher.js")
                .Include("~/Frontend-Assembly/UCommerce.Sitefinity.UI/Mvc/Scripts/jsInit.js"));
        }
    }
}
