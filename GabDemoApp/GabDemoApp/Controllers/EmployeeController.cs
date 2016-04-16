using System.Threading.Tasks;
using System.Web.Mvc;
using GabDemoApp.Models;
using GabDemoApp.Service;

namespace GabDemoApp.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly EmployeeService _employeeService;

        public EmployeeController()
        {
            _employeeService = new EmployeeService();
        }

       
        public ActionResult Index()
        {
            var employees = _employeeService.GetEmployees();
            return View(employees);
        }

        public ActionResult Details(string id)
        {
            var employee = _employeeService.GetEmployee(id);
            return View(employee);
        }

        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public  async Task<ActionResult> Create(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _employeeService.CreateEmployeeAsync(employee);
                    return RedirectToAction("Index");

                }
                return View(employee);
            }
            catch
            {
                return View(employee);
            }
        }

       
        public ActionResult Edit(string id)
        {
            var employee = _employeeService.GetEmployee(id);
            return View(employee);
        }

        
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _employeeService.UpdateEmployeeAsync(employee);
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch
            {
                return View(employee);
            }
        }

        public ActionResult Delete(string id)
        {
            var employee = _employeeService.GetEmployee(id);
            return View(employee);
        }

        [HttpPost]
        public async Task<ActionResult>  Delete(int id, Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _employeeService.DeleteEmployeeAsyc(employee);
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch
            {
                return View(employee);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _employeeService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
