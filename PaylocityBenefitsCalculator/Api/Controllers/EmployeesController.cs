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
                    int numDependents = HelperFunctions.GetAllDependents(employees).Count();
                    
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


        [SwaggerOperation(Summary = "Update employee and dependents of employee by id")]
        [HttpPut("UpdateDependents/{id}")]
        public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> UpdateEmployeeAndDependents(int id, GetEmployeeDto updatedEmployeeWithDependents)
        {
            var employees = HelperFunctions.GetAllEmployees();
            var originalEmployee = HelperFunctions.GetEmployeeGivenId(id, employees);

            // Date of Birth should never change, don't update it
            originalEmployee.FirstName = updatedEmployeeWithDependents.FirstName;
            originalEmployee.LastName = updatedEmployeeWithDependents.LastName;
            originalEmployee.Salary = updatedEmployeeWithDependents.Salary;

            // When deleting, will need to keep track of which IDs were deleted. Don't want to update IDs until all records are updated.
            List<int> deletedIds = new List<int>();

            // If the employee had dependents, it's possible that they need to be updated
            if(originalEmployee.Dependents != null && originalEmployee.Dependents.Count > 0)
            {
                // Get number of dependents in case new ones need to be added
                var numDependents = HelperFunctions.GetAllDependents(employees).Count();
                // first, delete any dependents that are in the original, but don't have a matching id in the updated
                foreach(var originalDependent in originalEmployee.Dependents)
                {
                    bool matchFound = false;
                    // Searching for a matching id. If one is found, record was not deleted.
                    // If one is not found, record was deleted
                    foreach(var updatedDependent in updatedEmployeeWithDependents.Dependents)
                    {
                        if (updatedDependent.Id == originalDependent.Id)
                        {
                            matchFound = true;
                            break;
                        }
                    }
                    if (!matchFound)
                    {
                        // Can't delete here since we're looping through the collection
                        deletedIds.Add(originalDependent.Id);
                    }
                }
                
                // Delete dependents
                for(int i = 0; i < deletedIds.Count; i++)
                {
                    var targetForDeletion = HelperFunctions.GetDependentGivenId(deletedIds[i], employees);
                    originalEmployee.Dependents.Remove(targetForDeletion);
                }

                // for every dependent given, either update them or add them.
                foreach(var updatedDependent in updatedEmployeeWithDependents.Dependents)
                {
                    // if the updated record has an id, then it must have a matching record
                    // The UI passes back ID of 0 as a flag to be a new dependent
                    if(updatedDependent.Id > 0)
                    {
                        foreach(var original in originalEmployee.Dependents)
                        {
                            if(original.Id == updatedDependent.Id)
                            {
                                original.FirstName = updatedDependent.FirstName;
                                original.LastName = updatedDependent.LastName;
                                if(HelperFunctions.CheckIfSpouseOrPartnerExistsExcludingId(originalEmployee, updatedDependent.Id))
                                {
                                    var failedResult = new ApiResponse<GetEmployeeDto>
                                    {
                                        Message = "Can only have one spouse or domestic partner, trying to add more",
                                        Error = "Only one spouse or partner allowed",
                                        Success = false
                                    };
                                    return failedResult;
                                }
                                else
                                {
                                    original.Relationship = updatedDependent.Relationship;
                                }
                                break;
                            }
                            
                        }
                    }
                    // Otherwise it's a new dependent that needs a new id
                    else
                    {
                        numDependents++;
                        updatedDependent.Id = numDependents;
                        if(updatedDependent.Relationship == Relationship.Spouse || updatedDependent.Relationship == Relationship.DomesticPartner)
                        {
                            if (HelperFunctions.CheckIfSpouseOrPartnerExists(originalEmployee))
                            {
                                var failedResult = new ApiResponse<GetEmployeeDto>
                                {
                                    Message = "Can only have one spouse or domestic partner, trying to add more",
                                    Error = "Only one spouse or partner allowed",
                                    Success = false
                                };
                                return failedResult;
                            }
                        }
                        originalEmployee.Dependents.Add(updatedDependent);
                    }
                }
            }
            // If they didn't, just accept the new ones, then assign IDs to them
            else
            {
                var numDependents = HelperFunctions.GetAllDependents(employees).Count();
                originalEmployee.Dependents = updatedEmployeeWithDependents.Dependents;
                foreach(var dependent in originalEmployee.Dependents)
                {
                    numDependents++;
                    dependent.Id = numDependents;
                }
            }

            // Update Dependent Ids if neccessary
            if(deletedIds.Count > 0)
            {
                // sort the ids and reverse to make sure we start with the highest one
                deletedIds.Sort();
                deletedIds.Reverse();

                // for each one deleted, we need to lower the id of each dependent with a greater id by one.
                for(int i = 0; i < deletedIds.Count; i++)
                {
                    // Update IDs beyond this one
                    IEnumerable<GetDependentDto> dependentsAsCollection =
                        from dependentObject in HelperFunctions.GetAllDependents(employees)
                        where dependentObject.Id > deletedIds[i]
                        select dependentObject;
                    foreach (GetDependentDto dependentDto in dependentsAsCollection)
                    {
                        dependentDto.Id -= 1;
                    }
                }
            }

            // Save the new state
            HelperFunctions.SerializeEmployeeCollection(employees);

            var result = new ApiResponse<GetEmployeeDto>
            {
                Data = originalEmployee,
                Message = "Employee And Dependents Updated",
                Success = true
            };
            return result;
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
