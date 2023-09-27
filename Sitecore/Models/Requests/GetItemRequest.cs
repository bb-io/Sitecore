using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Models.Requests
{
    public class GetItemRequest
    {
        [Display("Item ID")]
        public string Id { get; set; }

        [Display("Database")]
        public string? Database { get; set; }
    }
}
