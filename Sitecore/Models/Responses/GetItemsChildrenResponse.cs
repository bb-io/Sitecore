using Sitecore.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Models.Responses
{
    public class GetItemsChildrenResponse
    {
        public List<ItemDto> Items { get; set; }
    }
}
