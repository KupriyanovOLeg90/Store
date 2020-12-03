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
                pageList = db.Pages.ToArray().OrderBy(x=>x.Sorting).Select(x=> new PageVM(x)).ToList();
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
                else if (db.Pages.Any(x =>x.Slag == slag))
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
    }
}