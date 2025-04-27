using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearnApps.ViewModel.FFTS
{
   public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserId")]
        public int UserId { set; get; }
        public string UserName { set; get; }

    }
}
