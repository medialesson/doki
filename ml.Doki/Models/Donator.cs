using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace ml.Doki.Models
{
    public class Donator
    {
        public string FullName { get; set; }

        public ImageSource AvatarSource { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
