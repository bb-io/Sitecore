using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Dtos
{
    public class ItemDto
    {
        [Display("Item ID")]
        public string ItemID { get; set; }

        [Display("Item name")]
        public string ItemName { get; set; }

        [Display("Item path")]
        public string ItemPath { get; set; }

        [Display("Parent ID")]
        public string ParentID { get; set; }

        [Display("Template ID")]
        public string TemplateID { get; set; }

        [Display("Template name")]
        public string TemplateName { get; set; }

        [Display("Clone source")]
        public string CloneSource { get; set; }

        [Display("Item language")]
        public string ItemLanguage { get; set; }

        [Display("Item version")]
        public string ItemVersion { get; set; }

        [Display("Display name")]
        public string DisplayName { get; set; }

        [Display("Has children")]
        public string HasChildren { get; set; }

        [Display("Item icon")]
        public string ItemIcon { get; set; }

        [Display("Item media URL")]
        public string ItemMedialUrl { get; set; }

        [Display("Item URL")]
        public string ItemUrl { get; set; }

        public string Text { get; set; }
        public string Title { get; set; }
    }
}
