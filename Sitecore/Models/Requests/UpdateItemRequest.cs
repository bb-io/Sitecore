using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Models.Requests
{
    public class UpdateItemRequest
    {
        [Display("Item ID")]
        public string ItemID { get; set; }

        [Display("Database")]
        public string? Database { get; set; }

        [Display("Item name")]
        public string? ItemName { get; set; }

        [Display("Item language")]
        public string? ItemLanguage { get; set; }

        [Display("Display name")]
        public string? DisplayName { get; set; }

        [Display("Item icon")]
        public string? ItemIcon { get; set; }
        public string? Text { get; set; }
        public string? Title { get; set; }
    }
}
