using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        //GET - INDEX
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategories);
        }

        //GET - CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),

            };

            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if(doesSubCategoryExists.Count() > 0)
                {
                    //ERROR
                    StatusMessage = "Error : this subcategory does already exist under the name " + doesSubCategoryExists.First().Category.Name + ". Please use another name";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
                //if the modelstate is not valid, we might have values that are null in the model, so we create a new object and return it
                SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel
                {
                    CategoryList = await _db.Category.ToListAsync(),
                    SubCategory = model.SubCategory,
                    SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                    StatusMessage = StatusMessage

                };
                return View(modelVM);
        }
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {

            List<SubCategory> subCategories2 = new List<SubCategory>();
            //subCategories.Re All();
            subCategories2 = await (from subCategory in _db.SubCategory
                             where subCategory.CategoryId == id
                             select subCategory).ToListAsync();
            //Return the list as a select list
            return Json(new SelectList(subCategories2, "Id", "Name"));
    }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if(subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),

            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if (doesSubCategoryExists.Count() > 0)
                {
                    //ERROR
                    StatusMessage = "Error : this subcategory does already exist under the name " + doesSubCategoryExists.First().Category.Name + ". Please use another name";
                }
                else
                {
                    var editedSubcategory = await _db.SubCategory.FindAsync(id);
                    editedSubcategory.Name = model.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            //if the modelstate is not valid, we might have values that are null in the model, so we create a new object and return it
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage

            };
            modelVM.SubCategory.Id = id;
            return View(modelVM);
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }


            return View(subCategory);
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }


            return View(subCategory);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubCategory(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var subCategoryToDelete = await _db.SubCategory.FindAsync(id);
            _db.SubCategory.Remove(subCategoryToDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

    