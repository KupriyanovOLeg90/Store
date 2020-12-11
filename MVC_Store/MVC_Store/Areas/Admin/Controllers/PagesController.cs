using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        [HttpGet]
        public ActionResult Index()
        {
            List<PageVM> pageList;

            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }



            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //объявляем переменную для краткого описания (slag)
                string slag;

                //Инициализируем класс PageDTO
                PagesDTO pagesDTO = new PagesDTO();
                //Присвоем заголовок модели
                pagesDTO.Title = model.Title.ToUpper();
                //Проверям есть ли краткое описание если нет то добавляем его
                if (string.IsNullOrWhiteSpace(model.Slag))
                    slag = model.Title.Replace(" ", "-").ToLower();
                else
                    slag = model.Slag.Replace(" ", "-").ToLower();

                //Проверить заголовок и краткое опичание на уникальность
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title allready exist");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slag == slag))
                {
                    ModelState.AddModelError("", "That slag allready exist");
                    return View(model);
                }
                //Присваиваем оставшиеся значения модели
                pagesDTO.Slag = slag;
                pagesDTO.HasSidebar = model.HasSidebar;
                pagesDTO.Body = model.Body;
                pagesDTO.Sorting = 100;

                //Сохраняем в БД
                db.Pages.Add(pagesDTO);
                db.SaveChanges();
            }

            //Передаем сообщение через ТемпДата
            TempData["SM"] = "You have added new Page";

            //Переадрисовываем на метод Index
            return RedirectToAction("Index");
        }

        // Get: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Объявим модель
            PageVM pageVM;

            using (Db db = new Db())
            {
                //Получаем страницу 
                PagesDTO pagesDTO = db.Pages.Find(id);
                //Проверяем доступна ли страница 
                if (pagesDTO == null)
                    return Content("Page not found");
                //Инициализируем модель данными
                pageVM = new PageVM(pagesDTO);
            }
            //Возвращаем представление с моделью
            return View(pageVM);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //объявляем переменную для краткого описания (slag)
                string slag;
                int id = model.Id;

                //Получаем класс PageDTO
                PagesDTO pagesDTO = db.Pages.Find(id);

                if (pagesDTO == null)
                    return Content("Page not found");

                //Присвоем заголовок модели
                pagesDTO.Title = model.Title.ToUpper();

                //Проверям есть ли краткое описание если нет то добавляем его
                if (string.IsNullOrWhiteSpace(model.Slag))
                    slag = model.Title.Replace(" ", "-").ToLower();
                else
                    slag = model.Slag.Replace(" ", "-").ToLower();

                //Проверить заголовок и краткое опиcание на уникальность
                if (db.Pages.Where(x=>x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title allready exist");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slag == slag))
                {
                    ModelState.AddModelError("", "That slag allready exist");
                    return View(model);
                }
                //Присваиваем оставшиеся значения модели
                pagesDTO.Slag = slag;
                pagesDTO.HasSidebar = model.HasSidebar;
                pagesDTO.Body = model.Body;

                //Сохраняем в БД
                db.SaveChanges();
            }

            //Передаем сообщение через ТемпДата
            TempData["SM"] = "You have editing Page";

            //Переадрисовываем на метод Index
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            //Объявим модель
            PageVM pageVM;

            using (Db db = new Db())
            {
                //Получаем страницу 
                PagesDTO pagesDTO = db.Pages.Find(id);
                //Проверяем доступна ли страница 
                if (pagesDTO == null)
                    return Content("Page not found");
                //Инициализируем модель данными
                pageVM = new PageVM(pagesDTO);
            }
            //Возвращаем представление с моделью
            return View(pageVM);
        }

        // Get: Admin/Pages/EditPage
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Получаем страницу 
                PagesDTO pagesDTO = db.Pages.Find(id);
                //Проверяем доступна ли страница 
                if (pagesDTO == null)
                    return Content("Page not found");

                db.Pages.Remove(pagesDTO);
                db.SaveChanges();
            }

            //Передаем сообщение через ТемпДата
            TempData["SM"] = "You have delete Page";

            //Переадрисовываем на метод Index
            return RedirectToAction("Index");
        }

        //Метод сортировки
        // Post: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {            
                int count = 0;

                //Получаем страницу 
                PagesDTO pagesDTO;

                foreach (var pageId in id) 
                {
                    pagesDTO = db.Pages.Find(pageId);
                    pagesDTO.Sorting = count++;

                    db.SaveChanges();
                }
            }
        }


        //Метод сортировки
        // Get: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar() 
        {
            //Объявим модель
            SidebarVM sidebarVM;

            using (Db db = new Db())
            {
                //Получаем даннфи 
                SideBarDTO sidebarDTO = db.SideBars.Find(1); //TODO убрать 1 
                //Проверяем доступны ли данные 
                if (sidebarDTO == null)
                    return Content("Page not found");
                //Инициализируем модель данными
                sidebarVM = new SidebarVM(sidebarDTO);
            }

            //Возвращаем представление с моделью
            return View(sidebarVM);
        }


        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            //проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                int id = model.Id;
                //Получаем класс PageDTO
                SideBarDTO sideBarDTO = db.SideBars.Find(id);

                if (sideBarDTO == null)
                    return Content("Page not found");

                //Присваиваем значения модели
                sideBarDTO.Body = model.Body;

                //Сохраняем в БД
                db.SaveChanges();
            }

            //Передаем сообщение через ТемпДата
            TempData["SM"] = "You have editing Sidebar";

            //Переадрисовываем на метод EditSidebar
            return RedirectToAction("EditSidebar");
        }
    }
}