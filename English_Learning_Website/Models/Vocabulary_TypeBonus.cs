using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using English_Learning_Website.Models;

namespace English_Learning_Website.Models
{
    public class Vocabulary_TypeBonus
    {
        [Required(ErrorMessage = "Please enter Vocabulary Type Name")]
        [MaxLength(20, ErrorMessage = "Vocabulary Type Name no longer than 20 character")]
        public string Vocabulary_Type_EN_Name { get; set; }
        public int Vocabulary_Type_Code { get; set; }

        public List<Vocabulary_Type> vocabulary_Types { get; set; }
    }
}