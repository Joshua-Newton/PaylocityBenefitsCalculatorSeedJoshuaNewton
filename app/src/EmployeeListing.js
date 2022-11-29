import React, { useState, useEffect, useRef } from 'react';
import Employee from './Employee';
import { baseUrl } from './Constants';
import AddEmployeeModal from './AddEmployeeModal';
import DependentDetails from "./DependentDetails"

const EmployeeListing = () => {
    const [employees, setEmployees] = useState([]);
    const [error, setError] = useState(null);
    const [currentEmployee, setCurrentEmployee] = useState([]);
    const [currentId, setCurrentId] = useState('');
    const [currentDependents, setCurrentDependents] = useState([]);
    // Fields for Employee values.
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [dateOfBirth, setDateOfBirth] = useState('');
    const [salary, setSalary] = useState('');
    const [numDependents, setNumDependents] = useState('');

    const [dependentFields, setDependentFields] = useState([
        {LastName: '', FirstName: '', DateOfBirth: '', relationship: ''}
    ])

    // Refs for loading in existing values
    const lastNameRef = useRef(null);
    const firstNameRef = useRef(null);
    const dateOfBirthRef = useRef(null);
    const salaryRef = useRef(null);
    const numDependentsRef = useRef(null);

    // change handlers
    const handleFirstNameChange = event => {
        setFirstName(event.target.value);
    }
    const handleLastNameChange = event => {
        setLastName(event.target.value);
    }
    const handleDateOfBirthChange = event => {
        setDateOfBirth(event.target.value);
    }
    const handleSalaryChange = event => {
        setSalary(event.target.value);
    }
    const handleNumDependentsChange = event => {
        setNumDependents(event.target.value);
    }

    // For new employee
    const SubmitEmployee = async () => {
        const employeeData = {
            FirstName: firstName,
            LastName: lastName,
            DateOfBirth: dateOfBirth,
            Salary: salary,
            Dependents: []
        }

        const result = await fetch(`${baseUrl}/api/v1/Employees/`, {
            method: 'POST',
            headers: {
                'Content-Type' : 'application/json'
            },
            body: JSON.stringify(employeeData),
            type: 'json'
        })

        const resultInJSON = await result.json();
        console.log(resultInJSON);
    }

    //---------------------------------------------------------

    async function setEmployee(employeeId){
        if(employeeId){
            console.log("Passed ID: " + employeeId)
        }
        if(currentEmployee != null){
            console.log("Before: " + currentEmployee.id)
        }
        else {
            console.log("Before: null")
        }



        const rawEmployee = await fetch(`${baseUrl}/api/v1/Employees/${employeeId}`, {
            method: 'GET',
            headers: {
                'Content-Type' : 'application/json'
            }
        });
        /*
        const rawDependents = await fetch(`${baseUrl}/api/v1/Employees/GetDependents/${employeeId}`, {
            method: 'GET',
            headers: {
                'Content-Type' : 'application/json'
            }
        });
        */
        const responseEmployee = await rawEmployee.json();
        //const responseDependents = await rawDependents.json();

        if(responseEmployee.success){
            setCurrentEmployee(responseEmployee.data);
            setCurrentId(responseEmployee.data.id);
            setCurrentDependents(responseEmployee.data.dependents)
            lastNameRef.current.value = responseEmployee.data.lastName;
            firstNameRef.current.value = responseEmployee.data.firstName;
            dateOfBirthRef.current.value = responseEmployee.data.dateOfBirth.split('T')[0];
            salaryRef.current.value = responseEmployee.data.salary;
            numDependentsRef.current.value = responseEmployee.data.dependents.length;
        }
        else {
            setCurrentEmployee(null);
            setCurrentId(null);
            lastNameRef.current.value = "";
            firstNameRef.current.value = "";
            dateOfBirthRef.current.value = null;
            salaryRef.current.value = "";
            
        }

        /*
        if(responseDependents.success){
            setCurrentDependents((responseDependents.data));
            if(responseDependents.data){
                numDependentsRef.current.value = responseDependents.data.length;
            }
            else {
                numDependentsRef.current.value = 0;
            }
        }
        else {
            setCurrentDependents(null);
            numDependentsRef.current.value = 0;
        }
        */

        if(currentEmployee != null){
            console.log("After: " + currentEmployee.id)
        }
        else {
            console.log("After: null")
        }    
    }

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

    useEffect(() => {
        console.log("Set Current Employee Ran: ")
        if(currentEmployee){
            console.log(currentEmployee);
        }
        else {
            console.log("current employee null");
        }
    }, [currentEmployee])


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
                    currentEmployee={currentEmployee}
                    setEmployee={setEmployee}
                />
            ))}
            </tbody>
        </table>
        <button onClick={() => setEmployee(0)} type="button" className="btn btn-primary" data-bs-toggle="modal" data-bs-target={`#${addEmployeeModalId}`}>Add Employee</button>
        {/* Former AddEmployeeModal, was having trouble passing refs so I pulled it back into the EmployeeListing. */}
        <div className="modal fade" id="add-employee-modal" tabIndex="-1" aria-labelledby="add-employee-modal-label" aria-hidden="true">
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h1 className="modal-title fs-5" id="add-employee-modal-label">Add/Edit Employee</h1>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        <div className="row">
                            <div className="col">
                                <h4>Employee Details</h4>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>LastName</label>
                            </div>
                            <div className="col-6">
                                <input id="idLastName" ref={lastNameRef} type={"text"} onChange={handleLastNameChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>FirstName</label>
                            </div>
                            <div className="col-6">
                                <input id="idFirstName" ref={firstNameRef} type={"text"} onChange={handleFirstNameChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>DOB</label>
                            </div>
                            <div className="col-6">
                                <input id="idDOB" ref={dateOfBirthRef} type={"date"} onChange={handleDateOfBirthChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>Salary</label>
                            </div>
                            <div className="col-6">
                                <input id="idSalary" ref={salaryRef} type={"number"} onChange={handleSalaryChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>Number of Dependents</label>
                            </div>
                            <div className="col-6">
                                <input ref={numDependentsRef} id="numDependents" onChange={handleNumDependentsChange}></input>
                            </div>
                        </div>

                        {currentDependents == null ? null : currentDependents.map(({id, firstName, lastName, dateOfBirth, relationship}) => (
                        <DependentDetails
                            key={id}
                            id={id}
                            firstName={firstName}
                            lastName={lastName}
                            dateOfBirth={dateOfBirth}
                            relationship={relationship}
                            editModalId={addEmployeeModalId}
                            currentEmployee={currentEmployee}
                        />
                    ))}
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" className="btn btn-primary" onClick={SubmitEmployee}>Save changes</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    );
};

export default EmployeeListing;