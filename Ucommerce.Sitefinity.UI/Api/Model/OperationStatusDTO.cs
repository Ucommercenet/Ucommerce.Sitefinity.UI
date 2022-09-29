using System.Collections.Generic;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    public class OperationStatusDTO
    {
        public Dictionary<string, object> Data { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

        public OperationStatusDTO()
        {
            Data = new Dictionary<string, object>();
        }
    }
}
