using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using English_Learning_Website.Models;

namespace English_Learning_Website.Models
{
    public class StoryBonus
    {
        [DisplayName("Story Name : ")]
        [Required(ErrorMessage = " Please enter story name ")]
        [MaxLength(50)]
        public string Story_Name { get; set; }
        [DisplayName("Story Content English : ")]
        [Required(ErrorMessage = " Please enter story content english ")]
        [MaxLength(50)]
        public string Story_Content_EN { get; set; }
        [DisplayName("Story Content Vietnamese : ")]
        [Required(ErrorMessage = " Please enter story content vietnamese ")]
        [MaxLength(50)]
        public string Story_Content_VN { get; set; }
        [DisplayName("Story Audio : ")]
        [Required(ErrorMessage = " Please enter story audio ")]
        [MaxLength(50)]
        public string Story_Audio { get; set; }
        [DisplayName("Story banner : ")]
        [Required(ErrorMessage = " Please enter story banner ")]
        [MaxLength(50)]
        public string Story_banner { get; set; }
        [DisplayName("Story image : ")]
        [Required(ErrorMessage = " Please enter story image ")]
        [MaxLength(50)]
        public string Story_image { get; set; }
        public int Story_Code { get; set; }
        public string Userz_FullName { get; set; }
        public Nullable<int> Userz_Code { get; set; }

        public virtual Userz Userz { get; set; }
    }
}