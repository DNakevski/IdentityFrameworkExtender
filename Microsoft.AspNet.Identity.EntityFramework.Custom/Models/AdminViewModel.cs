using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "CategoryName")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int? TranslationCode { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
        public IEnumerable<SelectListItem> CategoriesList { get; set; }
    }
}