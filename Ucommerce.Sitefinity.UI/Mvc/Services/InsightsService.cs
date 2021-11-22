using System;
using System.Linq;
using System.Web;
using Telerik.DigitalExperienceCloud.Client;
//using Telerik.Sitefinity.DataIntelligenceConnector.Managers;
using Ucommerce.Api;
using Ucommerce.Extensions;
using Ucommerce.Infrastructure;
using Ucommerce.Search.Facets;
using UCommerce.Sitefinity.UI.Search;

namespace UCommerce.Sitefinity.UI.Mvc.Services
{
	public interface IInsightsService
	{
		void SendAsSentence(Ucommerce.EntitiesV2.Category category, string predicate, string objectName);
		void SendAsSentence(Ucommerce.Search.Models.Category category, string predicate, string objectName);
		void SendAsSentence(Ucommerce.EntitiesV2.Product product, string predicate, string objectName);
		void SendAsSentence(Ucommerce.Search.Models.Product product, string predicate, string objectName);
		void SendAsSentence(Ucommerce.EntitiesV2.Customer customer, string predicate, string objectName);
		void SendAsSentence(Ucommerce.EntitiesV2.PurchaseOrder order, string predicate, string objectName);
	}

	public class InsightsService : IInsightsService
	{
		// Name:Sitefinity.Ucommerce.UI DEV
		private const string dataCenterKey = "564712e4-50f1-caa1-e877-0afdd4b08099";
		private readonly ITransactionLibrary _transactionLibrary = ObjectFactory.Instance.Resolve<ITransactionLibrary>();
		//private readonly IInteractionManager _interactionManager = ObjectFactory.Instance.Resolve<IInteractionManager>(); // Telerik.Sitefinity.DataIntelligenceConnector

		public void SendAsSentence(Ucommerce.EntitiesV2.Category category, string predicate, string objectName)
		{
			var interaction = CreateInteractionForBasket(predicate, objectName);
			AddBasketInteractions(interaction);
			AddCategoryInteractions(interaction, category);
			ImportInteraction(interaction);
		}

		public void SendAsSentence(Ucommerce.Search.Models.Category category, string predicate, string objectName)
		{
			var interaction = CreateInteractionForBasket(predicate, objectName);
			AddBasketInteractions(interaction);
			AddCategoryInteractions(interaction, category);
			ImportInteraction(interaction);
		}

		public void SendAsSentence(Ucommerce.EntitiesV2.Product product, string predicate, string objectName)
		{
			var interaction = CreateInteractionForBasket(predicate, objectName);
			AddBasketInteractions(interaction);
			AddProductInteractions(interaction, product);
			ImportInteraction(interaction);
		}

		public void SendAsSentence(Ucommerce.Search.Models.Product product, string predicate, string objectName)
		{
			var interaction = CreateInteractionForBasket(predicate, objectName);
			AddBasketInteractions(interaction);
			AddProductInteractions(interaction, product);
			ImportInteraction(interaction);
		}

		public void SendAsSentence(Ucommerce.EntitiesV2.Customer customer, string predicate, string objectName)
		{
			var interaction = CreateInteractionForBasket(predicate, objectName);
			AddBasketInteractions(interaction);
			AddCustomerInteractions(interaction, customer);
			ImportInteraction(interaction);
		}

		public void SendAsSentence(Ucommerce.EntitiesV2.PurchaseOrder order, string predicate, string objectName)
		{
			var interaction = GetInteractionForOrder(objectName, predicate, order);
			AddOrderInteractions(interaction, order);
			ImportInteraction(interaction);
		}

		private Interaction CreateInteractionForBasket(string predicate, string objectName)
		{
			var order = SafeGetOrder();
			return GetInteractionForOrder(predicate, objectName, order);
		}

		private Ucommerce.EntitiesV2.PurchaseOrder SafeGetOrder()
		{
			Ucommerce.EntitiesV2.PurchaseOrder order = null;
			try
			{
				order = _transactionLibrary.GetBasket();
			}
			catch (Exception ex)
			{
				// TODO: Log
			}

			return order;
		}

		private Interaction GetInteractionForOrder(string predicate, string objectName, Ucommerce.EntitiesV2.PurchaseOrder order)
		{
			var subject = order?.OrderGuid.ToString() ?? FakeBasketId();
			var interaction = new Interaction()
			{
				Subject = subject, // TODO: Change this to the tracking cookie from the API IInteractionManager
								   //Subject = _interactionManager.GetTrackingId(),
				Predicate = predicate,
				Object = objectName
			};
			return interaction;
		}

		private string FakeBasketId()
		{
			var cookieName = "tempBasketId";

			var basketId = HttpContext.Current.Request.Cookies[cookieName]?.Value;
			if (!string.IsNullOrWhiteSpace(basketId)) return basketId;

			basketId = Guid.NewGuid().ToString();

			var cookie = new HttpCookie(cookieName)
			{
				Value = basketId
			};
			cookie.Expires.Add(TimeSpan.FromDays(30));
			HttpContext.Current.Response.Cookies.Add(cookie);

			return basketId;
		}

		private static void ImportInteraction(Interaction interaction)
		{
			if (interaction == null) return;

			var interactionClient = new InteractionClient(dataCenterKey);
			interactionClient.ImportInteraction("ucommerce", interaction).Wait();
		}

		private void AddBasketInteractions(Interaction interaction)
		{
			if (interaction == null) return;

			var order = SafeGetOrder();
			AddOrderInteractions(interaction, order);
		}

		private void AddCategoryInteractions(Interaction interaction, Ucommerce.EntitiesV2.Category category)
		{
			if (interaction == null) return;

			if (category == null) return;

			const string prefix = "Category";
			AddObjectMetaData(interaction, prefix, "CategoryId", category.CategoryId);
			AddObjectMetaData(interaction, prefix, "CategoryGuid", category.Guid);
			AddObjectMetaData(interaction, prefix, "Name", category.Name);
			AddObjectMetaData(interaction, prefix, "DisplayName", category.DisplayName());

			foreach (var property in category.CategoryProperties)
				AddObjectMetaData(interaction, prefix, $"Property{property.DefinitionField.Name}", property.Value);

			AddFacets(interaction);
		}

		private void AddCategoryInteractions(Interaction interaction, Ucommerce.Search.Models.Category category)
		{
			if (interaction == null) return;

			if (category == null) return;

			const string prefix = "Category";
			AddObjectMetaData(interaction, prefix, "CategoryGuid", category.Guid);
			AddObjectMetaData(interaction, prefix, "Name", category.Name);
			AddObjectMetaData(interaction, prefix, "DisplayName", category.DisplayName);

			foreach (var property in category.GetUserDefinedFields())
				AddObjectMetaData(interaction, prefix, $"Property{property.Key}", property.Value);

			AddFacets(interaction);
		}

		private void AddProductInteractions(Interaction interaction, Ucommerce.EntitiesV2.Product product)
		{
			if (interaction == null) return;

			if (product == null) return;

			const string prefix = "Product";
			AddObjectMetaData(interaction, prefix, "ProductId", product.ProductId);
			AddObjectMetaData(interaction, prefix, "ProductGuid", product.Guid);
			AddObjectMetaData(interaction, prefix, "Name", product.Name);
			AddObjectMetaData(interaction, prefix, "DisplayName", product.DisplayName());

			foreach (var property in product.ProductProperties)
				AddObjectMetaData(interaction, prefix, $"Property_{property.ProductDefinitionField.Name}", property.Value);

			AddFacets(interaction);
		}

		private void AddProductInteractions(Interaction interaction, Ucommerce.Search.Models.Product product)
		{
			if (interaction == null) return;

			if (product == null) return;

			const string prefix = "Product";
			AddObjectMetaData(interaction, prefix, "ProductGuid", product.Guid);
			AddObjectMetaData(interaction, prefix, "Name", product.Name);
			AddObjectMetaData(interaction, prefix, "DisplayName", product.DisplayName);

			foreach (var property in product.GetUserDefinedFields())
				AddObjectMetaData(interaction, prefix, $"Property_{property.Key}", property.Value);

			AddFacets(interaction);
		}

		private static void AddOrderInteractions(Interaction interaction, Ucommerce.EntitiesV2.PurchaseOrder order)
		{
			if (interaction == null) return;

			if (order == null) return;

			const string prefix = "Order";
			AddObjectMetaData(interaction, prefix, "CultureCode", order.CultureCode);
			AddObjectMetaData(interaction, prefix, "BasketId", order.BasketId.ToString());
			AddObjectMetaData(interaction, prefix, "OrderGuid", order.OrderGuid.ToString());
			AddObjectMetaData(interaction, prefix, "BillingCurrency", order.BillingCurrency.ToString());

			foreach (var property in order.OrderProperties)
				AddObjectMetaData(interaction, prefix, $"Property_{property.Key}", property.Value);

			AddAddressInteractions(interaction, order.BillingAddress);
			AddCustomerInteractions(interaction, order.Customer);
		}

		private static void AddCustomerInteractions(Interaction interaction, Ucommerce.EntitiesV2.Customer customer)
		{
			if (interaction == null) return;

			if (customer == null) return;

			interaction.SubjectMetadata.Add("Email", customer.EmailAddress);
		}

		private static void AddAddressInteractions(Interaction interaction, Ucommerce.EntitiesV2.IAddress address)
		{
			if (interaction == null) return;

			if (address == null) return;

			interaction.SubjectMetadata.Add("FirstName", address.FirstName);
			interaction.SubjectMetadata.Add("LastName", address.LastName);
			interaction.SubjectMetadata.Add("Email", address.EmailAddress);
			interaction.SubjectMetadata.Add("Country", address.Country.Name);
			interaction.SubjectMetadata.Add("CountryISO", address.Country.TwoLetterISORegionName);
		}

		private void AddFacets(Interaction interaction)
		{
			if (interaction == null) return;

			var facets = HttpContext.Current.Request.QueryString.ToFacets();
			if (!(facets?.Any() ?? false)) return;

			const string prefix = "Facet";
			foreach (var facet in facets)
				AddFacetValues(interaction, prefix, facet);
		}

		private static void AddFacetValues(Interaction interaction, string prefix, Facet facet)
		{
			if (interaction == null) return;

			// NOTE: We can't currently send through multiple values with the same id
			//foreach (var value in facet.FacetValues)
			//	AddSubjectMetaData(interaction, prefix, facet.Name, value.Value);
			// AddObjectMetaData(interaction, prefix, facet.Name, string.Join("|", facet.FacetValues.Select(f => f.Value)));
			foreach (var value in facet.FacetValues)
				AddObjectMetaData(interaction, prefix, $"{facet.Name}_{value.Value}".ToLower(), "true");
		}

		private static void AddObjectMetaData(Interaction interaction, string prefix, string key, object valueToAdd)
		{
			if (interaction == null) return;

			if (valueToAdd == null) return;

			var value = valueToAdd.ToString();

			if (string.IsNullOrWhiteSpace(value)) return;

			interaction.ObjectMetadata.Add($"{prefix}{key}", value);
		}
	}
}
