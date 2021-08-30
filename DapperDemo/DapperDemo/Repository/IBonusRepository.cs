using DapperDemo.Models;
using System.Collections.Generic;

namespace DapperDemo.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmploteeWithCompany(int id);
        Company GetCompanyWithEmployees(int id);
        List<Company> GetAllCompanyWithEmployees();
        void AddTestCompanyWithEmployees(Company objCompany);
        void RemoveRange(int[] companyId);
        List<Company> FilterCompanyByName(string name);
        void AddTestCompanyWithEmployeesWithTransaction(Company objCompany);
        
    }
}
