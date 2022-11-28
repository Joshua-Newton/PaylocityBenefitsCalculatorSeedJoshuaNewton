import { useState } from "react";
import { baseUrl } from "./Constants";

const AddEmployeeModal = (props) => {

    // Fields for Employee values. Exclude ID as api handles assigning this value.
    const [firstName, setFirstName] = useState('')
    const [lastName, setLastName] = useState('')
    const [dateOfBirth, setDateOfBirth] = useState('')
    const [salary, setSalary] = useState('')
    
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

    return (
        <div className="modal fade" id="add-employee-modal" tabIndex="-1" aria-labelledby="add-employee-modal-label" aria-hidden="true">
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h1 className="modal-title fs-5" id="add-employee-modal-label">Add/Edit Employee</h1>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        <div className="row">
                            <div className="col-6">
                                <label>LastName</label>
                            </div>
                            <div className="col-6">
                                <input id="idLastName" type={"text"} onChange={handleLastNameChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>FirstName</label>
                            </div>
                            <div className="col-6">
                                <input id="idFirstName" type={"text"} onChange={handleFirstNameChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>DOB</label>
                            </div>
                            <div className="col-6">
                                <input id="idDOB" type={"date"} onChange={handleDateOfBirthChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>Salary</label>
                            </div>
                            <div className="col-6">
                                <input id="idSalary" type={"number"} onChange={handleSalaryChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>Number of Dependents</label>
                            </div>
                            <div className="col-6">
                                <input id="numDependents"></input>
                            </div>
                        </div>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" className="btn btn-primary" onClick={SubmitEmployee}>Save changes</button>
                    </div>
                </div>
            </div>
        </div>
    );
};


export default AddEmployeeModal;