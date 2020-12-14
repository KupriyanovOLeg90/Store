using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{pages}
        public ActionResult Index(string page = "")
        {
            //Установить краткий загаловок 
            if (page == string.Empty)
                page = "home";

            //Объявляем модель и данные
            PageVM model;
            PagesDTO dto;

            //Проверяем доступна ли текущая страница
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slag.Equals(page)))
                    return RedirectToAction("Index", new { page = "" });
            }

            //Получаем контекст данных этой страницы
            using (Db db = new Db())
            {
                dto = db.Pages.FirstOrDefault(x => x.Slag == page);
            }
            //Устанавливаем заголовок
            ViewBag.PageTitle = dto.Title;

            //Проверяем боковую панель
            if (dto.HasSidebar)
                ViewBag.Sidebar = "yes";
            else
                ViewBag.Sidebar = "no";

            //Заполнить модель данными
            model = new PageVM(dto);

            //Вернуть представление в месте с моделью
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            List<PageVM> pageVMs;

            using (Db db = new Db())
            {
                pageVMs = db.Pages.ToArray().Where(x=>x.Title != "home")
                            .OrderBy(x=>x.Sorting)
                            .Select(x=>new PageVM(x)).ToList();
            }


            return PartialView("_PagesMenuPartial", pageVMs);
        }

        public ActionResult SidebarPartial()
        {
            SidebarVM model;

            using (Db db = new Db())
            {
                var sb = db.SideBars.FirstOrDefault();
                model = new SidebarVM(sb);
            }


            return PartialView("_SidebarPartial", model);
        }
    }
}