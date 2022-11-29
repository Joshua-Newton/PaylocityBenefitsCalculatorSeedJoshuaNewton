const DependentDetails = (props) => {
    const firstName = props.firstName || '';
    const lastName = props.lastName || '';
    const relationship = props.relationship || 0;
    const dateOfBirth = props.dateOfBirth.split('T')[0] || null;

    const possibleRelationships = ["none", "spouse", "domesticPartner", "child"]
    function DetermineRelationship() {
        return possibleRelationships[relationship];
    }



    return (
        <div className="DependentDetails">
            <div className="row">
                <div className="col">
                    <h4>Dependent</h4>
                </div>
            </div>
            <div className="row">
                <div className="col-6">
                    <label>First Name</label>
                </div>
                <div className="col-6">
                    <input value={firstName} type={"text"}></input>
                </div>
            </div>
            <div className="row">
                <div className="col-6">
                    <label>Last Name</label>
                </div>
                <div className="col-6">
                    <input value={lastName} type={"text"}></input>
                </div>
            </div>
            <div className="row">
                <div className="col-6">
                    <label>DateOfBirth</label>
                </div>
                <div className="col-6">
                    <input value={dateOfBirth} type={"date"}></input>
                </div>
            </div>
            <div className="row">
                <div className="col-6">
                    <label>Relationship</label>
                </div>
                <div className="col-6">
                    <select value={DetermineRelationship()}>
                        <option value="none">None</option>
                        <option value="spouse">Spouse</option>
                        <option value="domesticPartner">Domestic Partner</option>
                        <option value="child">Child</option>
                    </select>
                </div>
            </div>
        </div>
    );
};

export default DependentDetails;