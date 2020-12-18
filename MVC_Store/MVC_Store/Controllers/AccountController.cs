using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Account;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_Store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM user)
        {
            //Проверить модель не валидность
            if (!ModelState.IsValid)
                return View("CreateAccount", user);

            //Проверить соответствие пароля
            if (!user.Password.Equals(user.ConfirmPassword))
            {
                ModelState.AddModelError("", "Passwort do not match!");
                return View("CreateAccount", user);
            }

            //Проверить имя на уникальность
            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(user.Username)))
                {
                    ModelState.AddModelError("", $"User name '{user.Username}' is tacken.");
                    user.Username = "";
                    return View("CreateAccount", user);
                }

                //создать экземпляр DTO
                UserDTO userDTO = new UserDTO();
                userDTO.FirstName = user.FirstName;
                userDTO.LastName = user.LastName;
                userDTO.Password = user.Password;
                userDTO.EmailAdress = user.EmailAdress;
                userDTO.Username = user.Username;

                //добавить данные
                db.Users.Add(userDTO);

                //сохранить данные
                db.SaveChanges();

                //добавляем роль
                UserRoleDTO role = new UserRoleDTO();
                role.UserId = userDTO.Id;
                role.RoleId = 2;

                db.UserRoles.Add(role);
                db.SaveChanges();

            }
            //Записать сообщение в ТD
            TempData["SM"] = "You are now registered and can login.";

            //Переадрисовываем пользователя
            return RedirectToAction("Login");
        }

        // GET: account/login
        [HttpGet]
        public ActionResult Login()
        {
            //Подтвердить что пользователь не авторизован
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");

            //Возвращаем представление
            return View();
        }

        // POST: account/login
        [HttpPost]
        public ActionResult Login(LoginUserVM user)
        {
            //Проверить модель не валидность
            if (!ModelState.IsValid)
                return View(user);

            //Проверяем пользователя на валидность
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(user.Username)
                                 && x.Password.Equals(user.Password)))
                {
                    isValid = true;
                }

                if (isValid == false)
                {
                    ModelState.AddModelError("", "Invalid user name or password.");
                    return View(user);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(user.Username, user.RememberMe));
                }
            }
        }

        // GET: account/logout
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        public ActionResult UserNavPartial()
        {
            //Получить имя пользователя
            string username = User.Identity.Name;

            //Объявить модель
            UserNavPartialVM model = new UserNavPartialVM();

            //Получаем пользователя
            using (Db db = new Db())
            {
                //Заполняем модель данными
                var user = db.Users.FirstOrDefault(x => x.Username == username);

                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
            }

            //Возвращаем частично представление с моделью

            return PartialView("_UserNavPartial", model);
        }

        // GET: account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            //получить имя пользователя
            string username = User.Identity.Name;

            //объявить модель
            UserProfileVM model;

            //получаем пользователя
            using (Db db = new Db())
            {
                //Заполняем модель данными
                var user = db.Users.FirstOrDefault(x => x.Username == username);

                //инициализируем данными
                model = new UserProfileVM(user);
            }

            //возвращаем модель в представление
            return View("UserProfile", model);
        }
        // POST: account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameisChanged = false;

            if (!ModelState.IsValid)
                return View("UserProfile", model);

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwort do not match!");
                    return View("UserProfile", model);
                }
            }

            using (Db db = new Db())
            {
                string userName = User.Identity.Name;

                if (userName != model.Username)
                {
                    userNameisChanged = true;
                    userName = model.Username;
                }

                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"User name '{model.Username}' is tacken.");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                var user = db.Users.Find(model.Id);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.EmailAdress = model.EmailAdress;
                user.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                    user.Password = model.Password;


                db.SaveChanges();
            }

            TempData["SM"] = "You are edited profile.";

            if (userNameisChanged)
                return RedirectToAction("Logout");

            return View("UserProfile", model);
        }

    }
}