using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Events;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Web;
using Ucommerce.Sitefinity.UI.Mvc.Controllers;

namespace Ucommerce.Sitefinity.UI.Pages
{
    internal static class HttpContextExtensions
    {
        public static string GetValue<T>(this Dictionary<string, string> context, Expression<Func<T, object>> expression)
            where T : Controller, new()
        {
            var propertyName = GetMemberName(expression.Body);
            if (!string.IsNullOrEmpty(propertyName) && context != null && context.ContainsKey(propertyName))
            {
                return context[propertyName];
            }

            return null;
        }

        public static bool TryGetValue<T>(this Dictionary<string, string> context, Expression<Func<T, object>> expression, out string result)
            where T : Controller, new()
        {
            var isSuccess = false;
            result = string.Empty;
            var propertyName = GetMemberName(expression.Body);
            if (!string.IsNullOrEmpty(propertyName) && context != null && context.ContainsKey(propertyName))
            {
                result = context[propertyName];
                if (!string.IsNullOrEmpty(result))
                    isSuccess = true;
            }

            return isSuccess;
        }

        private static void LoadToContextDictionary<T>(this PageData pageData, Expression<Func<T, object>> expression, Dictionary<string, string> contextDictionary)
            where T : Controller, new()
        {
            if (contextDictionary == null)
                throw new ArgumentNullException(nameof(contextDictionary));

            var controllerName = typeof(T).FullName;
            var propertyName = GetMemberName(expression.Body);

            var propertyValue = GetPropertyValueInternal(pageData, controllerName, propertyName);

            if (!string.IsNullOrEmpty(propertyValue))
            {
                if (contextDictionary.ContainsKey(propertyName))
                    contextDictionary[propertyName] = propertyValue;
                else
                    contextDictionary.Add(propertyName, propertyValue);
            }
        }

        private static string GetPropertyValue<T>(this PageData pageData, Expression<Func<T, object>> expression)
            where T : Controller, new()
        {
            var controllerName = typeof(T).FullName;
            var propertyName = GetMemberName(expression.Body);

            return GetPropertyValueInternal(pageData, controllerName, propertyName);
        }

        internal static void PublishinManager_Executing(object sender, ExecutingEventArgs e)
        {

            if (e.CommandName == "CommitTransaction" || e.CommandName == "FlushTransaction")
            {
                try
                {
                    var provider = sender as PageDataProvider;
                    var dirtyItems = provider.GetDirtyItems();
                    if (dirtyItems.Count != 0)
                    {
                        foreach (var item in dirtyItems)
                        {
                            var pageData = item as PageData;
                            var url = System.Web.HttpContext.Current.Request.Url.ToString();

                            if (IsValidPublishingOperation(item, pageData, provider))
                            {
                                var contextDictionary = new Dictionary<string, string>();
                                var isManualSelectionMode = pageData.GetPropertyValue<ProductsController>(p => p.IsManualSelectionMode);
                                if (!string.IsNullOrEmpty(isManualSelectionMode) && bool.TryParse(isManualSelectionMode, out bool result) && result == true)
                                {
                                    pageData.LoadToContextDictionary<ProductsController>(x => x.CategoryIds, contextDictionary);
                                    pageData.LoadToContextDictionary<ProductsController>(x => x.ProductIds, contextDictionary);
                                }

                                pageData.NavigationNode.SetValue(PAGE_CONTEXT_FIELD_NAME, JsonConvert.SerializeObject(contextDictionary));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write($"Page context was not set due to unhandled exception, which occured during the page publishing event handler. The following exception was thrown: {Environment.NewLine} {ex}", ConfigurationPolicy.ErrorLog);
                }
            }
        }

        public static Dictionary<string, string> GetProductsContext(this HttpContextBase httpContext)
        {
            var contextDictionary = new Dictionary<string, string>();
            var smp = SiteMapBase.GetCurrentProvider();
            var pageDataId = ((PageSiteNode)smp.CurrentNode).PageId;

            var mgr = PageManager.GetManager();
            var pageData = mgr.GetPageDataList()
                .Where(pd => pd.Id == pageDataId && pd.Status == ContentLifecycleStatus.Live && pd.Visible == true)
                .FirstOrDefault();

            if (pageData != null)
            {
                pageData.LoadToContextDictionary<ProductsController>(x => x.CategoryIds, contextDictionary);
                pageData.LoadToContextDictionary<ProductsController>(x => x.ProductIds, contextDictionary);
            }

            return contextDictionary;
        }

        private static string GetPropertyValueInternal(PageData pageData, string controllerName, string propertyName)
        {
            if (pageData == null)
                throw new ArgumentNullException(nameof(pageData));

            var result = string.Empty;
            var mvcWidget = pageData.Controls
                                  .Where(c => c.IsLayoutControl == false && c.ObjectType == "Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy" && c.Properties.Any(p => p.Name == "ControllerName" && p.Value == controllerName))
                                  .FirstOrDefault();

            if (mvcWidget != null)
            {
                var widgetSettings = mvcWidget.Properties.Where(p => p.Name == "Settings").FirstOrDefault();
                if (widgetSettings != null)
                {
                    var childProperties = widgetSettings.ChildProperties;
                    var property = childProperties.Where(c => c.Name == propertyName).SingleOrDefault();
                    if (property != null && !string.IsNullOrEmpty(property.Value))
                    {
                        result = property.Value;
                    }
                }
            }

            return result;
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (expression is MemberExpression)
            {
                var memberExpression = expression as MemberExpression;
                return memberExpression.Member?.Name;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.Operand is MemberExpression)
                return ((MemberExpression)unaryExpression.Operand).Member?.Name;

            return null;
        }

        private static bool IsValidPublishingOperation(object dirtyItem, PageData pageData, PageDataProvider pageDataProvider)
        {
            var result = false;

            if (pageData != null && pageDataProvider != null)
            {
                SecurityConstants.TransactionActionType itemStatus = pageDataProvider.GetDirtyItemStatus(dirtyItem);
                if (itemStatus == SecurityConstants.TransactionActionType.Updated)
                {
                    var url = System.Web.HttpContext.Current.Request.Url.ToString();
                    if (url.Contains("workflowOperation=Publish") || url.Contains("/batchPublishDraft/"))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public const string PAGE_CONTEXT_FIELD_NAME = "UcommerceContext";
    }
}
