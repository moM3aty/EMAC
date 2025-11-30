using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMAC.ViewModels
{
    public class ServicePageViewModel
    {
        public List<SelectListItem> ServicesList { get; set; }
        public List<SelectListItem> LocationsList { get; set; }

        public ServicePageViewModel()
        {
            ServicesList = new List<SelectListItem>();
            LocationsList = new List<SelectListItem>();
        }
    }
}