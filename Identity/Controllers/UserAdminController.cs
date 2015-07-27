using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [PermissionAuthorize(Permissions.Admin)]
    public class UsersAdminController : BaseIdendityController
    {
        public UsersAdminController()
        {
        }

        #region // <Index>

        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        #endregion // </Index>

        #region // <Details>

        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            ViewBag.CategoryNames = await CategoryManager.GetCategoriesAsync(user.Id);

            return View(user);
        }

        #endregion // </Details>

        #region // <Create>

        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            var model = new RegisterViewModel();
            model.CategoriesList = new SelectList(await CategoryManager.Categories.ToListAsync(), "Name", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var succeded = true;
                var user = new CustomUser { UserName = model.Email, Email = model.Email };
                var adminresult = await UserManager.CreateAsync(user, model.Password);
                if (!adminresult.Succeeded)
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                }
                else
                {
                    user = await UserManager.FindByEmailAsync(model.Email);
                    var result = await UserManager.AddToCategoryAsync(user.Id, model.Category);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            model.CategoriesList = new SelectList(await CategoryManager.Categories.ToListAsync(), "Name", "Name");
            return View(model);
        }

        #endregion // </Create>

        #region // <Edit>

        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var userCategories = await CategoryManager.GetCategoriesAsync(user.Id);
            var category = user.CategoryUsers.Select(u => u.Category).FirstOrDefault();

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                }),
                CategoriesList = CategoryManager.Categories.ToList().Select(x => new SelectListItem()
                {
                    Selected = userCategories.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                CategoryId = category != null ? category.Id : -1
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,SelectedCategory")] EditUserViewModel model)// params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                var category = await CategoryManager.FindByIdAsync(model.CategoryId);
                if (category == null)
                {
                    return HttpNotFound();
                }

                user.UserName = model.Email;
                user.Email = model.Email;

                /*
                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());
                */

                var result = await UserManager.AddToCategoryAsync(user.Id, category.Name);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        #endregion // </Edit>

        #region // <Delete>

        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        #endregion // </Delete>
    }
}
