﻿using EmployeeManager.Data;
using EmployeeManager.Models;
using EmployeeManager.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace EmployeeManager.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeDbContext _employeeContext;
        public EmployeesController(EmployeeDbContext employeeDbContext)
        {
            this._employeeContext = employeeDbContext;
        }

        [HttpGet]
        public IActionResult Index(string sortOrder, string searchString)
        {   

            var employees = from s in _employeeContext.Employees
                           select s;

            employees = _sortHelper(sortOrder, employees);

            if(!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.LastName.Contains(searchString));
            }

            return View(employees.ToList());
        }
        
        private IQueryable<Employee> _sortHelper(string sortOrder, IQueryable<Employee> employees)
        {
            var newEmployees = employees;

            ViewBag.FirstNameParm = (String.IsNullOrEmpty(sortOrder) || sortOrder == "first_asc") ? "first_desc" : "first_asc";
            ViewBag.LastNameParm = (String.IsNullOrEmpty(sortOrder) || sortOrder == "last_asc") ? "last_desc" : "last_asc";
            ViewBag.MiddleNameParm = (String.IsNullOrEmpty(sortOrder) || sortOrder == "mid_asc") ? "mid_desc" : "mid_asc";
            ViewBag.AddressParm = (String.IsNullOrEmpty(sortOrder) || sortOrder == "add_asc") ? "add_desc" : "add_asc";
            ViewBag.DOBParm = (String.IsNullOrEmpty(sortOrder) || sortOrder == "dob_asc") ? "dob_desc" : "dob_asc";
            ViewBag.SSNParm = (String.IsNullOrEmpty(sortOrder) || sortOrder == "ssn_asc") ? "ssn_desc" : "ssn_asc";

            var orderCommands = new Dictionary<string, Action>()
            {
                { "first_desc", () => newEmployees = employees.OrderByDescending(x => x.FirstName)},
                { "first_asc", () => newEmployees = employees.OrderBy(x => x.FirstName)},
                { "last_desc", () => newEmployees = employees.OrderByDescending(x => x.LastName) },
                { "last_asc", () => newEmployees = employees.OrderBy(x => x.LastName)},
                { "mid_desc", () => newEmployees = employees.OrderByDescending(x => x.MiddleName)},
                { "mid_asc", () => newEmployees = employees.OrderBy(x => x.MiddleName)},
                { "add_desc", () => newEmployees = employees.OrderByDescending(x => x.Address)},
                { "add_asc", () => newEmployees = employees.OrderBy(x => x.Address)},
                { "dob_desc", () => newEmployees = employees.OrderByDescending(x => x.DateOfBirth)},
                { "dob_asc", () => newEmployees = employees.OrderBy(x => x.DateOfBirth)},
                { "ssn_desc", () => newEmployees = employees.OrderByDescending(x => x.SocialSecurityNumber)},
                { "ssn_asc", () => newEmployees = employees.OrderBy(x => x.SocialSecurityNumber)}
            };

            if (!sortOrder.IsNullOrEmpty() && orderCommands.ContainsKey(sortOrder))
                orderCommands[sortOrder].Invoke();

            return newEmployees;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest) 
        {

            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                FirstName = addEmployeeRequest.FirstName,
                MiddleName = addEmployeeRequest.MiddleName,
                LastName = addEmployeeRequest.LastName,
                Address = addEmployeeRequest.Address,
                DateOfBirth = addEmployeeRequest.DateOfBirth,
                SocialSecurityNumber = addEmployeeRequest.SocialSecurityNumber,
                DateCreated = DateTime.Now,
            };

            await _employeeContext.Employees.AddAsync(employee);
            await _employeeContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var employee = await _employeeContext.Employees.FirstOrDefaultAsync(x =>x.Id == id);
            if(employee == null)
            {
                return RedirectToAction("Index");
            }

            var selectedEmployee = new UpdateEmployeeViewModel()
            {
                Id = employee.Id,
                FirstName = employee.FirstName.Trim(),
                MiddleName = employee.MiddleName.Trim(),
                LastName = employee.LastName.Trim(),
                Address = employee.Address.Trim(),
                DateOfBirth = employee.DateOfBirth,
                SocialSecurityNumber = employee.SocialSecurityNumber,

            };
            return View(selectedEmployee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateEmployeeViewModel updateEmployeeRequest)
        {
            var employee = await _employeeContext.Employees.FindAsync(updateEmployeeRequest.Id);

            if (employee != null)
            {
                employee.FirstName = updateEmployeeRequest.FirstName.Trim();
                employee.MiddleName = updateEmployeeRequest.MiddleName.Trim();
                employee.LastName = updateEmployeeRequest.LastName.Trim();
                employee.Address = updateEmployeeRequest.Address.Trim();
                employee.DateOfBirth = updateEmployeeRequest.DateOfBirth;
                employee.SocialSecurityNumber = updateEmployeeRequest.SocialSecurityNumber;

                await _employeeContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if(id == null)
            {
                return BadRequest();
            }

            var employee = await _employeeContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            var selectedEmployee = new UpdateEmployeeViewModel()
            {
                Id = employee.Id,
                FirstName = employee.FirstName.Trim(),
                MiddleName = employee.MiddleName.Trim(),
                LastName = employee.LastName.Trim(),
                Address = employee.Address.Trim(),
                DateOfBirth = employee.DateOfBirth,
                SocialSecurityNumber = employee.SocialSecurityNumber,

            };
            return View(selectedEmployee);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel deleteEmployeeRequest)
        {
            var employee = await _employeeContext.Employees.FindAsync(deleteEmployeeRequest.Id);

            if (employee != null)
            {
                _employeeContext.Employees.Remove(employee);
                await _employeeContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}
