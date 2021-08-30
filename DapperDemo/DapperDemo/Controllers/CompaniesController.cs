using DapperDemo.Models;
using DapperDemo.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DapperDemo.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository _compRepo;
        private readonly IEmployeeRepository _empRepo;
        private readonly IBonusRepository _bonRepo;
        private readonly IDapperSprocRepo _dapRepo;

        public CompaniesController(ICompanyRepository compRepo, IEmployeeRepository empRepo, IBonusRepository bonRepo, IDapperSprocRepo dapRepo)
        {
            _compRepo = compRepo;
            _empRepo = empRepo;
            _bonRepo = bonRepo;
            _dapRepo = dapRepo;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            //return View(_compRepo.GetAll());
            return View(_dapRepo.List<Company>("usp_GetAllCompany"));
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _bonRepo.GetCompanyWithEmployees(id.GetValueOrDefault());

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (ModelState.IsValid)
            {
                _compRepo.Add(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _dapRepo.Single<Company>("usp_GetCompany", new { CompanyId = id.GetValueOrDefault() });
            //var company =  _compRepo.Find(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _compRepo.Update(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _compRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }
    }
}
