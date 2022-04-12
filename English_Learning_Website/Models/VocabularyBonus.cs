using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using English_Learning_Website.Models;
using PagedList;
using PagedList.Mvc;

namespace English_Learning_Website.Models
{
    public class VocabularyBonus
    {
        [Required(ErrorMessage = " Please choose vocabulary type ")]
        public List<Vocabulary_Type> vocabulary_Types { get; set; }
        public List<Vocabulary> vocabularies { get; set; }
        [DisplayName("English Name : ")]
        [Required(ErrorMessage = " Please enter english name ")]
        [MaxLength(30)]
        public string Vocabulary_English { get; set; }
        [DisplayName("Vietnamese Name : ")]
        [Required(ErrorMessage = " Please enter vietnamese name ")]
        [MaxLength(30)]
        public string Vocabulary_Vietnamese { get; set; }
        [DisplayName("Pronunciation : ")]
        [Required(ErrorMessage = " Please enter pronunciation ")]
        [MaxLength(20)]
        public string Vocabulary_Pronunciation { get; set; }
        [DisplayName("Vocabulary Image : ")]
        [Required(ErrorMessage = " Please enter vocabulary image ")]
        [MaxLength(50)]
        public string Vocabulary_Image { get; set; }
        public int Vocabulary_Type_Code { get; set; }
        [Required(ErrorMessage = "Please enter vocabulary type name")]
        [MaxLength(20, ErrorMessage = "Vocabulary Type Name can't longer than 20 character")]
        public string Vocabulary_Type_EN_Name { get; set; }
        public int Vocabulary_Code { get; set; }

    }
}