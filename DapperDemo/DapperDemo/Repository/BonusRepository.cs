using Dapper;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DapperDemo.Repository
{
    public class BonusRepository : IBonusRepository
    {
        private IDbConnection db;

        public BonusRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompanyWithEmployees(Company objCompany)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES (@Name, @Address, @City, @State, @PostalCode);"
                    + "SELECT CAST(SCOPE_IDENTITY() as int);";

            var id = db.Query<int>(sql, objCompany).Single();

            objCompany.CompanyId = id;

            //foreach (var employee in objCompany.Employees)
            //{
            //    employee.CompanyId = objCompany.CompanyId;

            //    var sql1 = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES (@Name, @Title, @Email, @Phone, @CompanyId);"
            //       + "SELECT CAST(SCOPE_IDENTITY() as int);";

            //    var id1 = db.Query<int>(sql1, employee).Single();
            //}

            objCompany.Employees.Select(c => { c.CompanyId = id; return c; }).ToList();

            var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES (@Name, @Title, @Email, @Phone, @CompanyId);"
               + "SELECT CAST(SCOPE_IDENTITY() as int);";

            db.Execute(sqlEmp, objCompany.Employees);

        }

        public List<Company> FilterCompanyByName(string name)
        {
            return db.Query<Company>("SELECT * FROM Companies WHERE Name LIKE '%' + @name + '%' ", new { name }).ToList();
        }

        public List<Company> GetAllCompanyWithEmployees()
        {
            var sql = "SELECT C.*, E.* FROM Employees AS E INNER JOIN Companies AS C ON E.CompanyId = C.CompanyId ";

            var companyDic = new Dictionary<int, Company>();

            var company = db.Query<Company, Employee, Company>(sql, (c, e) =>
            {
                if(!companyDic.TryGetValue(c.CompanyId, out var currentCompany)){
                    currentCompany = c;
                    companyDic.Add(currentCompany.CompanyId, currentCompany);
                 }

                currentCompany.Employees.Add(e);

                return currentCompany;
            }, splitOn: "EmployeeId");

            return company.Distinct().ToList();
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var p = new
            {
                CompanyId = id
            };
            var sql = "SELECT * FROM Companies WHERE CompanyId = @CompanyId;"
                + "SELECT * FROM Employees WHERE CompanyId = @CompanyId; ";
            
            Company company;

            using (var lists = db.QueryMultiple(sql, p)) 
            {
                company = lists.Read<Company>().ToList().FirstOrDefault();
                company.Employees = lists.Read<Employee>().ToList();
            }

            return company;
        }

        public List<Employee> GetEmploteeWithCompany(int id)
        {
            var sql = "SELECT E.*, C.* FROM Employees AS E INNER JOIN Companies AS C ON E.CompanyId = C.CompanyId ";

            if (id != 0)
            {
                sql += " WHERE E.CompanyId = @Id ";
            }


            var employee = db.Query<Employee, Company, Employee>(sql, (e, c) =>
            {
                e.Company = c;
                return e;
            },new { id } ,splitOn: "CompanyId");

            return employee.ToList();
        }

        public void RemoveRange(int[] companyId)
        {
            db.Query("DELETE FROM Companies WHERE CompanyId IN @companyId", new { companyId });
        }
    }
}
