const AddEmployeeModal = (props) => {
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
                                <input id="idInput" type={"text"}></input>
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
                        <button type="button" className="btn btn-primary">Save changes</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AddEmployeeModal;