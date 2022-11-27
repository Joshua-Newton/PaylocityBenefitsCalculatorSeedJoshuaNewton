import { useState } from "react";
import { baseUrl } from "./Constants";

const AddEmployeeModal = (props) => {

    const [ID, setID] = useState('')

    const handleChange = event => {
        setID(event.target.value);
    }
    
    const SubmitEmployee = async () => {
        const employeeData = {
            id: ID,
            firstName: "FirstName",
            lastName: "LastName",
            dateOfBirth: new Date(2020,0,12),
            salary: "80000",
            dependents: []
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
                            <div className="col-3">
                                <label>ID</label>
                            </div>
                            <div className="col-9">
                                <input id="idInput" type={"text"} onChange={handleChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col">
                                <label>LastName</label>
                            </div>
                            <div className="col-9">
                                <input id="idLastName" type={"text"}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col">
                                <label>FirstName</label>
                            </div>
                            <div className="col-9">
                                <input id="idFirstName" type={"text"}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col">
                                <label>DOB</label>
                            </div>
                            <div className="col-9">
                                <input id="idDOB" type={"date"}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col">
                                <label>Salary</label>
                            </div>
                            <div className="col-9">
                                <input id="idSalary" type={"number"}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col">
                                <label>Dependents</label>
                            </div>
                            <div className="col-9">
                                <input id="idDependents" list="dependentsList"></input>
                                <datalist id="dependentsList">
                                    <option value="dependent1"></option>
                                    <option value="dependent2"></option>
                                </datalist>
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