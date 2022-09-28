﻿namespace UCommerce.Sitefinity.UI.Api.Model
{
    /// <summary>
    /// DTO class used for updating an order item.
    /// </summary>
    public class UpdateLineItemDTO
    {
        public int NewQuantity { get; set; }
        public int OrderlineId { get; set; }
    }
}
