using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCommerce.Sitefinity.UI.Api.Model
{
    public class OperationStatusDTO
    {
        public OperationStatusDTO()
        {
            this.Data = new Dictionary<string, object>();
        }

        public string Status { get; set; }

        public string Message { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}
