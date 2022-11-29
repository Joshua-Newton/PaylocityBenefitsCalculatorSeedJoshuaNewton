using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/Employees")]
    public class EmployeesController : ControllerBase
    {
        private string jsonFilePath = "EmployeeData.json";

        [SwaggerOperation(Summary = "Get dependents of employee by id")]
        [HttpGet("GetDependents/{id}")]
        public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetDependents(int id)
        {
            var employees = HelperFunctions.GetAllEmployees();
            IEnumerable<GetEmployeeDto> employeesAsCollection =
                from employeeObject in employees
                where employeeObject.Id == id
                select employeeObject;
            GetEmployeeDto targetEmployee = employeesAsCollection.FirstOrDefault();

            if (targetEmployee != null)
            {
                var result = new ApiResponse<List<GetDependentDto>>()
                {
                    Data = targetEmployee.Dependents as List<GetDependentDto>,
                    Success = true,
                    Message = "Employee Found"
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<List<GetDependentDto>>()
                {
                    Success = false,
                    Message = "Employee Not Found"
                };
                return result;
            }

        }

        [SwaggerOperation(Summary = "Get employee by id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
        {
            var employees = HelperFunctions.GetAllEmployees();
            IEnumerable<GetEmployeeDto> employeesAsCollection = 
                from employeeObject in employees
                where employeeObject.Id == id
                select employeeObject;
            GetEmployeeDto targetEmployee = employeesAsCollection.FirstOrDefault();

            if(targetEmployee != null)
            {
                var result = new ApiResponse<GetEmployeeDto>()
                {
                    Data = targetEmployee,
                    Success = true,
                    Message = "Employee Found"
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<GetEmployeeDto>()
                {
                    Success = false,
                    Message = "Employee Not Found"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Get all employees")]
        [HttpGet("")]
        public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
        {
            var employees = HelperFunctions.GetAllEmployees();

            if (employees != null)
            {
                if(employees.Count > 0)
                {
                    var result = new ApiResponse<List<GetEmployeeDto>>
                    {
                        Data = employees,
                        Success = true
                    };
                    return result;
                }
                else
                {
                    var result = new ApiResponse<List<GetEmployeeDto>>()
                    {
                        Success = true,
                        Message = "No Employees Exist"
                    };
                    return result;
                }
            }
            else
            {
                var result = new ApiResponse<List<GetEmployeeDto>>()
                {
                    Success = false,
                    Message = "Employee JSON Parse failed"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Add employee")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<List<AddEmployeeDto>>>> AddEmployee(AddEmployeeDto newEmployee)
        {
            // Use a reusable helper method to get all employees
            var employees = HelperFunctions.GetAllEmployees();

            if(employees != null)
            {
                List<GetDependentDto> newEmployeeDependents = new List<GetDependentDto>();
                // Only perform dependents logic if dependents were passed in
                if (newEmployee.Dependents != null && newEmployee.Dependents.Count > 0)
                {
                    // Get the number of dependents. This will determine IDs of new dependents.
                    int numDependents = 0;
                    foreach (var employee in employees)
                    {
                        if (employee.Dependents != null)
                        {
                            foreach (var dependent in employee.Dependents)
                            {
                                numDependents++;
                            }
                        }
                    }
                    
                    bool spouseOrDomesticPartnerAdded = false;
                    foreach(var dependent in newEmployee.Dependents)
                    {
                        // Passed in dependents will be of type AddDependentDto, but we need to assigne the Id to each of these dependents
                        // So, take their data and apply it to a GetDependentDto, while checking for spouse/domestic partner rules
                        GetDependentDto dependentInstance = new GetDependentDto()
                        {
                            FirstName = dependent.FirstName,
                            LastName = dependent.LastName,
                            DateOfBirth = dependent.DateOfBirth
                        };
                        if(dependent.Relationship == Relationship.Spouse || dependent.Relationship == Relationship.DomesticPartner)
                        {
                            if(!spouseOrDomesticPartnerAdded)
                            {
                                // Haven't seen another spouse/domestic partner, can add this dependent safely. Set flag that we've seen one.
                                spouseOrDomesticPartnerAdded = true;
                                // numDependents is the number of currently existing dependents, so add to it first to get a new id
                                numDependents++;
                                dependentInstance.Id = numDependents;
                                dependentInstance.Relationship = dependent.Relationship;
                                newEmployeeDependents.Add(dependentInstance);
                            }
                            else
                            {
                                // There are too many spouse/domestic partners being added, return a failed response
                                var resultFromBadDependents = new ApiResponse<List<AddEmployeeDto>>
                                {
                                    Success = false,
                                    Message = "Only one spouse/domestic partner allowed",
                                    Error = "Invalid dependent set passed"
                                };
                                return resultFromBadDependents;
                            }
                        }
                        else
                        {
                            // Dependent is relationship none or child, can add as many as we want.
                            // numDependents is the number of currently existing dependents, so add to it first to get a new id
                            numDependents++;
                            dependentInstance.Id = numDependents;
                            dependentInstance.Relationship = dependent.Relationship;
                            newEmployeeDependents.Add(dependentInstance);
                        }
                    }
                }



                var newEmployeeRecord = new GetEmployeeDto()
                {
                    Id = employees.Count + 1,
                    FirstName = newEmployee.FirstName,
                    LastName = newEmployee.LastName,
                    Salary = newEmployee.Salary,
                    DateOfBirth = newEmployee.DateOfBirth,
                    Dependents = newEmployeeDependents
                };

                employees.Add(newEmployeeRecord);

                HelperFunctions.SerializeEmployeeCollection(employees);
                
                var result = new ApiResponse<List<AddEmployeeDto>>
                {
                    Data = new List<AddEmployeeDto>
                    {
                        newEmployee
                    },
                    Success = true
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<List<AddEmployeeDto>>()
                {
                    Success = false,
                    Message = "Employee JSON Parse failed"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Update employee")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> UpdateEmployee(int id, UpdateEmployeeDto updatedEmployee)
        {
            // Retrieve list of employees
            var employees = HelperFunctions.GetAllEmployees();

            // Retrieve the matching record
            GetEmployeeDto employeeAsDTO = HelperFunctions.GetEmployeeGivenId(id, employees);

            // If the matching record is not null, then we can update it.
            if(employeeAsDTO != null)
            {
                // Update the record
                employeeAsDTO.FirstName = updatedEmployee.FirstName;
                employeeAsDTO.LastName = updatedEmployee.LastName;
                employeeAsDTO.Salary = updatedEmployee.Salary;

                HelperFunctions.SerializeEmployeeCollection(employees);

                var result = new ApiResponse<GetEmployeeDto>
                {
                    Data = employeeAsDTO,
                    Message = "Employee Updated",
                    Success = true
                };
                return result;
            }
            // If the matching record is null, then it does not exist.
            else
            {
                var result = new ApiResponse<GetEmployeeDto>
                {
                    Message = "Employee of given id does not exist",
                    Success = false
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Delete employee")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> DeleteEmployee(int id)
        {
            var employees = HelperFunctions.GetAllEmployees();
            var targetEmployee = HelperFunctions.GetEmployeeGivenId(id, employees);
            employees.Remove(targetEmployee);

            // Update IDs beyond this one
            IEnumerable<GetEmployeeDto> employeesAsCollection =
                from employeeObject in employees
                where employeeObject.Id > id
                select employeeObject;
            foreach(GetEmployeeDto employeeDTO in employeesAsCollection)
            {
                employeeDTO.Id -= 1;
            }

            HelperFunctions.SerializeEmployeeCollection(employees);

            var result = new ApiResponse<List<GetEmployeeDto>>
            {
                Data = employees,
                Message = "Employee Deleted",
                Success = true
            };
            return result;
        }
    }
}
