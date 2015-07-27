using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace IdentitySample.Controllers
{
    [PermissionAuthorize(Permissions.Admin)]
    public class CategoryAdminController : BaseIdendityController
    {
        public CategoryAdminController()
        {
        }


        //
        // GET: /Categories/
        public ActionResult Index()
        {
            return View(CategoryManager.Categories);
        }

        //
        // GET: /Categories/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = await CategoryManager.FindByIdAsync(id);

            // get the list of users in this category
            var users = item.CategoryUsers.Select(u => u.User).ToList();
            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();

            // get the list of Categories in this category
            var roles = item.CategoryRoles.Select(u => u.Role).ToList();
            ViewBag.Roles = roles;
            ViewBag.RoleCount = roles.Count();

            return View(item);
        }

        //
        // GET: /Categories/Create
        public ActionResult Create()
        {
            var model = new CategoryViewModel();
            model.RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = false,
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

            return View(model);
        }

        //
        // POST: /Categories/Create
        [HttpPost]
        public async Task<ActionResult> Create(CategoryViewModel model, params int[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var result = await CategoryManager.CreateCustomAsync(model.Name, model.Description);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                }
                else
                {
                    selectedRole = selectedRole ?? new int[] { };
                    foreach (var roleId in selectedRole)
                    {
                        var roleResult = await RoleManager.AddToCategoryAsync(roleId, model.Name);
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", roleResult.Errors.First());
                        }
                    }

                    return RedirectToAction("Index");
                }
            }

            model.RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Selected = false,
                Text = x.Name,
                Value = x.Name
            });
            return View(model);
        }

        //
        // GET: /Categories/Edit/Admin
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await CategoryManager.FindByIdAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            var categoryRoles = item.CategoryRoles.Select(r => r.RoleId).ToList();

            var model = new CategoryViewModel();
            model.Id = item.Id;
            model.Name = item.Name;
            model.Description = item.Description;
            model.TranslationCode = item.TranslationCode;
            model.RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Selected = categoryRoles.Contains(x.Id),
                Text = x.Name,
                Value = x.Id.ToString()
            });

            return View(model);
        }

        //
        // POST: /Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id,Description")] CategoryViewModel model, params int[] selectedRole)
        {
            var categoryRoles = new List<int>();

            if (ModelState.IsValid)
            {
                var cat = await CategoryManager.FindByIdAsync(model.Id);
                var result = await CategoryManager.UpdateAsync(cat.Id, model.Name, model.Description, model.TranslationCode);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                }
                else
                {
                    selectedRole = selectedRole ?? new int[] { };
                    var roleResult = await RoleManager.AddToCategoryAndRemoveTheOthersAsync(selectedRole, model.Name);
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", roleResult.Errors.First());
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }

                categoryRoles = cat.CategoryRoles.Select(r => r.RoleId).ToList();
            }

            model.RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Selected = categoryRoles.Contains(x.Id),
                Text = x.Name,
                Value = x.Id.ToString()
            });
            return View(model);
        }

        //
        // GET: /Categories/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var item = await CategoryManager.FindByIdAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        //
        // POST: /Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var item = await CategoryManager.FindByIdAsync(id);
                if (item == null)
                {
                    return HttpNotFound();
                }

                IdentityResult result = null;
                if (deleteUser != null)
                {
                    result = await CategoryManager.DeleteAsync(item.Name);
                }
                else
                {
                    result = await CategoryManager.DeleteAsync(item.Name);
                }

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
