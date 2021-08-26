using DapperDemo.Models;
using System.Collections.Generic;

namespace DapperDemo.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmploteeWithCompany();
    }
}
