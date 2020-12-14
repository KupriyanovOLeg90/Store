using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            List<CategoryVM> categoryList;

            using (Db db = new Db())
            {
                categoryList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            return View(categoryList);
        }
        // POST: Admin/Shop/NewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Объявить Id
            string id = "";

            using (Db db = new Db())
            {
                //Проверяем категрию на уникальность
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                //Инициализируем DTO
                CategoryDTO category = new CategoryDTO();

                //Добавляем данные в модель
                category.Name = catName;
                category.Sorting = 100;
                category.Slug = catName.Replace(" ", "-").ToLower();

                //Сохранить
                db.Categories.Add(category);
                db.SaveChanges();

                //Получить id чтоб вернуть представлению  
                id = category.Id.ToString();
            }

            //вернуть id
            return id;
        }

        //Метод сортировки
        // Post: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 0;

                //Получаем страницу 
                CategoryDTO dto;

                foreach (var pageId in id)
                {
                    dto = db.Categories.Find(pageId);
                    dto.Sorting = count++;

                    db.SaveChanges();
                }
            }
        }

        // Get: Admin/Shop/DeleteCategory
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //Получаем категорию 
                CategoryDTO dto = db.Categories.Find(id);
                //Проверяем доступна ли категория 
                if (dto == null)
                    return Content("Category not found");

                db.Categories.Remove(dto);
                db.SaveChanges();
            }

            //Передаем сообщение через ТемпДата
            TempData["SM"] = "You have deleted Category";

            //Переадрисовываем на метод Index
            return RedirectToAction("Categories");
        }

        //Метод Обновление
        // Post: Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                //Проверить имя на уникальность
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                //Получаем класс ДТО
                var dto = db.Categories.Find(id);

                //Редактируем данные ДТО
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Сохраняем изменения
                db.SaveChanges();
            }

            //Возвращаем слово
            return "ОК";
        }

        //Добавление продукта
        // Get: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM product = new ProductVM();
            using (Db db = new Db())
            {
                product.Categories = new SelectList(db.Categories.ToList(), "id", "name");
            }

            return View(product);
        }

        //Добавление продукта
        // Post: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Проверить модель на валидность
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "name");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                //Проверить имя продукта на уникальность
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "id", "name");
                    ModelState.AddModelError("", "This name of products is taken");
                    return View(model);
                }
            }
            int productId = 0;
            using (Db db = new Db())
            {
                //инициализируем и сохраняем в базу модель
                ProductDTO product = new ProductDTO();
                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Price = model.Price;
                product.Description = model.Description;
                product.CategoryId = model.CategoryId;

                CategoryDTO cat = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = cat.Name;

                db.Products.Add(product);
                db.SaveChanges();

                productId = product.Id;
            }

            TempData["SM"] = "You have added a product";


            #region Upload Image

            //Создать все директории для хранения картинки
            var originalDirectory = new DirectoryInfo(
                string.Format($"{Server.MapPath(@"/")}Images\\Uploads"));

            string[] paths = new string[] {
                 Path.Combine(originalDirectory.ToString(), "Products"),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString()),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString()+ "\\Thumbs"),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString() + "\\Gallery"),
                 Path.Combine(originalDirectory.ToString(), "Products\\" + productId.ToString() + "\\Gallery\\Thumbs")
            };
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            //Проверяем был ли файл загружен
            if (file != null && file.ContentLength > 0)
            {
                //Проверяем рассширение
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg"
                     && ext != "image/gif" && ext != "image/xpng" && ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "id", "name");
                        ModelState.AddModelError("", "The image was not upload - Incorrect image type");
                        return View(model);
                    }
                }
                //Создаем переменную с именем изображения
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    //Сохраняем имя изображение в модель
                    ProductDTO product = db.Products.Find(productId);
                    product.ImageName = imageName;
                    db.SaveChanges();
                }

                //Назначаем пути к оригинальному пути и уменьшенному изображению
                var path = string.Format($"{paths[1]}\\{imageName}");
                var path2 = string.Format($"{paths[2]}\\{imageName}");

                //Сохранить оригинальное изображение
                file.SaveAs(path);

                //Создаем и сохраняем именьшенное изоброжение
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);

                img.Save(path2);
            }

            #endregion


            return RedirectToAction("Products");
        }


        //Добавление продукта
        // Get: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductVM> products;

            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                products = db.Products.ToArray()
                            .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                            .Select(x => new ProductVM(x)).ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "id", "name");


                ViewBag.SelectedCat = catId.ToString();
            }

            var onePageOfProducts = products.ToPagedList(pageNumber, 3);

            ViewBag.onePageOfProducts = onePageOfProducts;

            return View();
        }

        //Редактирование продукта
        // Get: Admin/Shop/EditProduct
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductVM model;

            using (Db db = new Db())
            {
                var productDTO = db.Products.Find(id);

                if (productDTO == null)
                    return Content("That product doest not exist");

                model = new ProductVM(productDTO);

                model.Categories = new SelectList(db.Categories.ToList(), "id", "name");

                model.GaleryImages = Directory.EnumerateFiles(Server.MapPath($"~/Images/Uploads/Products/{model.Id}/Gallery/Thumbs/"))
                    .Select(fn => Path.GetFileName(fn));
            }

            return View(model);
        }

        //Редактирование продукта
        // Post: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Получаем ID продука
            int id = model.Id;

            //Заполняем список категориями и изображениями
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "name");
            }
            model.GaleryImages = Directory.EnumerateFiles(
                        Server.MapPath($"~/Images/Uploads/Products/{model.Id}/Gallery/Thumbs/"))
                        .Select(fn => Path.GetFileName(fn));

            //Проверяем модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Проверяем имя продукта на уникальность
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != model.Id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Thar Product name is taken");
                    return View(model);
                }
            }

            //Обновить подукт в БД
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(model.Id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-");
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            //Установить сообщение в ТемпДата
            TempData["SM"] = "You have edited the product";

            #region Image upload
            //Проверить загружены ли файлы
            if (file != null && file.ContentLength > 0)
            {
                //Проверяем рассширение
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" && ext != "image/jpeg" && ext != "image/pjpeg"
                     && ext != "image/gif" && ext != "image/xpng" && ext != "image/png")
                {

                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not upload - Incorrect image type");
                        return View(model);
                    }
                }
                //Создать все директории для хранения картинки
                var originalDirectory = new DirectoryInfo(
                    string.Format($"{Server.MapPath(@"/")}Images\\Uploads"));

                //установить пути для загрузки
                string[] paths = new string[] {
                Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() ),
                Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs") };

                //Удалить существующие файлы в директориях
                DirectoryInfo dy1 = new DirectoryInfo(paths[0]);
                DirectoryInfo dy2 = new DirectoryInfo(paths[1]);

                foreach (var file1 in dy1.GetFiles()) 
                    file1.Delete();
                foreach (var file2 in dy2.GetFiles())
                    file2.Delete();


                //Сохраняем имя файла
                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);

                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                //Назначаем пути к оригинальному пути и уменьшенному изображению
                var path = string.Format($"{paths[0]}\\{imageName}");
                var path2 = string.Format($"{paths[1]}\\{imageName}");

                //Сохранить оригинальное изображение
                file.SaveAs(path);

                //Создаем и сохраняем именьшенное изоброжение
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);

                img.Save(path2);
            }


            #endregion

            return RedirectToAction("Products");
        }


        // Get: Admin/Shop/DeleteProduct
        public ActionResult DeleteProduct(int id)
        {
            using (Db db = new Db())
            {
                //Получаем категорию 
                ProductDTO dto = db.Products.Find(id);
                //Проверяем доступна ли категория 
                if (dto == null)
                    return Content("Product not found");

                db.Products.Remove(dto);
                db.SaveChanges();
            }

            //Удалить существующие файлы в директориях

            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"/")}Images\\Uploads\\"));
            string path = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            //Передаем сообщение через ТемпДата
            TempData["SM"] = "You have deleted Products";

            //Переадрисовываем на метод Index
            return RedirectToAction("Products");
        }

        //Добавление изображения в галерею
        // POst: Admin/Shop/SaveGalleryImages/Id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"/")}Images\\Uploads\\"));
            string path1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id +"\\Gallery");
            string pathThumbs = Path.Combine(originalDirectory.ToString(), "Products\\" + id + "\\Gallery\\Thumbs");

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if (file != null && file.ContentLength > 0)
                {
                    var pathSave = string.Format($"{path1}\\{file.FileName}");
                    var pathSave2 = string.Format($"{pathThumbs}\\{file.FileName}");

                    file.SaveAs(pathSave);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1, 1);
                    img.Save(pathSave2);
                }
            }
        }

        //Удаление изображения из галерею
        // POst: Admin/Shop/DeleteImage/Id
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            var path = Request.MapPath($"~/Images/Uploads/Products/{id}/Gallery/{imageName}");
            var pathThumbs = Request.MapPath($"~/Images/Uploads/Products/{id}/Gallery/Thumbs/{imageName}");

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            if (System.IO.File.Exists(pathThumbs))
                System.IO.File.Delete(pathThumbs);
        }
    }
}