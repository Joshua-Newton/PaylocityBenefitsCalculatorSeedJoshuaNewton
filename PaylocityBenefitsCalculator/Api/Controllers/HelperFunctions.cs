using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using System.Text.Json;

namespace Api.Controllers
{

    public static class HelperFunctions
    {
        private static string jsonFilePath = "EmployeeData.json";

        // Return a list of all employees in the json file, including all their dependents.
        public static List<GetEmployeeDto> GetAllEmployees()
        {
            // Employees will go into a list of GetEmployeeDTOs
            var employees = new List<GetEmployeeDto>();

            // Use a StreamReader to read the json out of EmployeeData.json
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                employees = JsonSerializer.Deserialize<List<GetEmployeeDto>>(json);
            }

            // return the deserialized collection
            // Don't bother checking for null, this will be handled by API methods and they may have different ways to handle it.
            // Null just means the deserialization failed, likely due to bad JSON. This shouldn't happen, but it's good to note.
            return employees;
        }

        // Serializes an employee list to the json file. This is a pseudo-database that holds the employee list, which in turn holds dependents.
        public static async void SerializeEmployeeCollection(List<GetEmployeeDto> employees)
        {
            // Self explanatory. Serialize the collection into json to save it.
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            using FileStream createStream = System.IO.File.Create(jsonFilePath);
            await JsonSerializer.SerializeAsync(createStream, employees, serializeOptions);
            await createStream.DisposeAsync();
        }

        // Returns a specific employee given an id and a list of employees.
        public static GetEmployeeDto GetEmployeeGivenId(int id, List<GetEmployeeDto> employees)
        {
            // Use LINQ to quickly get the record that matches the id given
            IEnumerable<GetEmployeeDto> employeeAsCollection =
                from employeeObject in employees
                where employeeObject.Id == id
                select employeeObject;

            // Retrieve the matching record
            GetEmployeeDto employeeAsDTO = employeeAsCollection.FirstOrDefault();

            return employeeAsDTO;
        }

        // Returns a specific dependent given its id and the list of employees to search in.
        public static GetDependentDto GetDependentGivenId(int dependentId, List<GetEmployeeDto> employees)
        {
            // This is not a good approach, it's a brute force approach, would much rather use LINQ, but this will work until I figure out what's going on with LINQ errors
            foreach(GetEmployeeDto employee in employees) 
            { 
                if(employee.Dependents != null && employee.Dependents.Count() > 0)
                {
                    foreach(GetDependentDto dependent in employee.Dependents)
                    {
                        if(dependent.Id == dependentId)
                        {
                            return dependent;
                        }
                    }
                }
            }
            return null;

            // Having issue where dependentAsCollection.FirstOrDefault is throwing null reference exception even when the dependent does exist....
            // Going to implement a different solution to get around this.
            /*
            IEnumerable<GetDependentDto> dependentAsCollection =
                from employeeObject in employees
                from dependentObject in employeeObject.Dependents
                where dependentObject.Id == id
                select dependentObject;
            if(dependentAsCollection != null)
            {
                GetDependentDto targetDependent = dependentAsCollection.FirstOrDefault();
                return targetDependent;
            }
            return null;
            */
        }

        // Returns a list of all existing dependents for all employees.
        public static List<GetDependentDto> GetAllDependents(List<GetEmployeeDto> employees) 
        {
            // Non Linq method:

            // Create the collection to be returned
            List<GetDependentDto> dependents = new List<GetDependentDto>();
            // Loop through every employee
            foreach(var employee in employees)
            {
                // If they have dependents, loop through them and add them to collection
                if(employee.Dependents != null && employee.Dependents.Count > 0) 
                { 
                    foreach(GetDependentDto dependent in employee.Dependents)
                    {
                        dependents.Add(dependent);
                    }
                }
            }
            // return the collection
            return dependents;

            // LINQ method. This causes issues due to not being able to convert results to a list for some reason.
            // Leaving here because using LINQ would be better than using a nested for loop if a good workaround is found.
            /*
            IEnumerable<GetDependentDto> dependentsAsCollection =
                from employeeObject in employees
                from dependentObject in employeeObject.Dependents
                select dependentObject;
            if(dependentsAsCollection != null)
            {
                // For some reason this:
                // dependentsAsCollection.ToList()
                // Is causing a null reference exception.
                // Working around with loops
                List<GetDependentDto> listOfDependents = new List<GetDependentDto>();
                bool endFound = false;
                int index = 0;
                while(!endFound)
                {
                    // This isn't a very good way to do this.... but dependentsAsCollection.ToList() failing is forcing my hand.
                    // TODO: Try to find a better way after the rest of the project is done.
                    try
                    {
                        listOfDependents.Add(dependentsAsCollection.ElementAt<GetDependentDto>(index));
                        index++;
                    }
                    catch (Exception ex)
                    {
                        endFound = true;
                        continue;
                    }
                }
                return listOfDependents;
            }
            return null;
            */
        }

        // Returns the employee object that contains the given dependentId in their dependents list
        public static GetEmployeeDto GetEmployeeDtoContainingDependentId(int dependentId, List<GetEmployeeDto> employees)
        {
            IEnumerable<GetEmployeeDto> targetEmployee =
                from employeeObject in employees
                from dependentObject in employeeObject.Dependents
                where dependentObject.Id == dependentId
                select employeeObject;
            return targetEmployee.FirstOrDefault();
        }

        // returns true if a spouse or partner is in the given employee's dependents list.
        // returns false if a spouse or partner is not in the dependents list.
        public static bool CheckIfSpouseOrPartnerExists(GetEmployeeDto employee)
        {
            foreach(GetDependentDto dependent in employee.Dependents)
            {
                if(dependent.Relationship == Relationship.DomesticPartner || dependent.Relationship == Relationship.Spouse)
                {
                    return true;
                }
            }
            return false;
        }

        // returns true if the spouse or partner exists in the given employee's dependents list, but skips the given dependent id.
        // returns false if the spouse or partner doesn't exist or the given ID is the only spouse or partner.
        public static bool CheckIfSpouseOrPartnerExistsExcludingId(GetEmployeeDto employee, int dependentId)
        {
            foreach (GetDependentDto dependent in employee.Dependents)
            {
                if (dependent.Id != dependentId && (dependent.Relationship == Relationship.DomesticPartner || dependent.Relationship == Relationship.Spouse))
                {
                    return true;
                }
            }
            return false;
        }
    } 
}
