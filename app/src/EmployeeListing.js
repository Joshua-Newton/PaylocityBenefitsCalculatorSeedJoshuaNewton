import { useState, useEffect } from 'react';
import Employee from './Employee';
import { baseUrl } from './Constants';
import AddEmployeeModal from './AddEmployeeModal';

const EmployeeListing = () => {
    const [employees, setEmployees] = useState([]);
    const [error, setError] = useState(null);

    useEffect(() => {
        async function getEmployees() {
            const raw = await fetch(`${baseUrl}/api/v1/Employees`, {
                method: 'GET',
                headers: {
                    'Content-Type' : 'application/json'
                }
            });
            const response = await raw.json();
            if (response.success) {
                setEmployees(response.data);
                setError(null);
            }
            else {
                setEmployees([]);
                setError(response.error);
            }
        };
        getEmployees();
    }, []);

    const addEmployeeModalId = "add-employee-modal";

    return (
    <div className="employee-listing">
        <table className="table caption-top">
            <caption>Employees</caption>
            <thead className="table-dark">
                <tr>
                    <th scope="col">Id</th>
                    <th scope="col">LastName</th>
                    <th scope="col">FirstName</th>
                    <th scope="col">DOB</th>
                    <th scope="col">Salary</th>
                    <th scope="col">Dependents</th>
                    <th scope="col">Actions</th>
                </tr>
            </thead>
            <tbody>
            {employees.map(({id, firstName, lastName, dateOfBirth, salary, dependents}) => (
                <Employee
                    key={id}
                    id={id}
                    firstName={firstName}
                    lastName={lastName}
                    dateOfBirth={dateOfBirth}
                    salary={salary}
                    dependents={dependents}
                    editModalId={addEmployeeModalId}
                />
            ))}
            </tbody>
        </table>
        <button type="button" className="btn btn-primary" data-bs-toggle="modal" data-bs-target={`#${addEmployeeModalId}`}>Add Employee</button>
        <AddEmployeeModal
            id={addEmployeeModalId}
        />
    </div>
    );
};

export default EmployeeListing;