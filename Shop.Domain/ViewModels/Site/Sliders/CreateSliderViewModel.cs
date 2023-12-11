﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Site.Sliders
{
    public class CreateSliderViewModel
    {
        public string SliderTitle { get; set; }

        public string SliderText { get; set; }

        public int Price { get; set; }

        public string Href { get; set; }

        public string TextBtn { get; set; }
    }
    public enum CreateSliderResult
    {
        ImageNotFound,
        Success
    }
}
