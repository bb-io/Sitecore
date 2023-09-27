using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Models.Requests
{
    public class CreateItemRequest
    {
        [Display("Item name")]
        public string ItemName { get; set; }

        [Display("Template ID")]
        public string TemplateID { get; set; }
        public string Title { get; set; }

        [Display("Item folder")]
        public string ItemFolder { get; set; }

        [Display("Database")]
        public string? Database { get; set; }
    }
}
