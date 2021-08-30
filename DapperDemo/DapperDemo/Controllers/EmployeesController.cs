using DapperDemo.Models;
using DapperDemo.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperDemo.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ICompanyRepository _compRepo;
        private readonly IEmployeeRepository _empRepo;
        private readonly IBonusRepository _bonRepo;

        [BindProperty]
        public Employee Employee { get; set; }

        public EmployeesController(ICompanyRepository compRepo, IEmployeeRepository empRepo, IBonusRepository bonRepo)
        {
            _compRepo = compRepo;
            _empRepo = empRepo;
            _bonRepo = bonRepo;
        }

        public async Task<IActionResult> Index(int companyId = 0)
        {
            //List<Employee> employees = _empRepo.GetAll();
            //foreach (Employee obj in employees)
            //{
            //    obj.Company = _compRepo.Find(obj.CompanyId);
            //}
            List<Employee> employees = _bonRepo.GetEmploteeWithCompany(companyId);
            return View(employees);
        }

        public IActionResult Create()
        {
            IEnumerable<SelectListItem> companyList = _compRepo.GetAll().Select(i => new SelectListItem 
            { 
                Text = i.Name,
                Value = i.CompanyId.ToString()
            });

            ViewBag.CompanyList = companyList;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                await _empRepo.AddAsync(Employee);
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee =  _empRepo.Find(id.GetValueOrDefault());
            IEnumerable<SelectListItem> companyList = _compRepo.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.CompanyId.ToString()
            });

            ViewBag.CompanyList = companyList;

            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != Employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _empRepo.Update(Employee);
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _empRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }
    }
}
