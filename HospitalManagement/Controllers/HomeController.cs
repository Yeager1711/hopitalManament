using BELibrary.Core.Entity;
using BELibrary.DbContext;
using System.Linq;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var test = dbContext.News.FirstOrDefault();
            //var testview = Mapper.Map<NewsViewModel>(test);

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                ViewBag.Doctors = workScope.Doctors.Include(x => x.Faculty).Take(8).ToList();

                var latestPosts = workScope.Articles.Query(x => !x.IsDelete).OrderByDescending(x => x.ModifiedDate).Take(5).ToList();
                ViewBag.LatestPosts = latestPosts;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Trang mô tả ứng dụng của bạn. ";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Trang liên hệ của bạn. ";

            return View();
        }

        public ActionResult E404()
        {
            ViewBag.Message = "Trang liên hệ của bạn. ";

            return View();
        }
    }
}