import React from 'react'

const PatientDataPanel = ({ patientData }) => {
    return(
        <div className="row">
            <div className="row" style={{height: 15}}></div>
            <div className="col-lg-2">
                <h4>{ patientData.name[0].family + ', ' + patientData.name[0].given[0] }</h4>
            </div>
            <div className="col-lg-3">
                <div className="col-lg-12">
                    <div className="row">
                        <div className="col-md-6">
                            <strong>Birthday:</strong>
                            <br />
                            <strong>Gender:</strong>
                            <br />
                            <strong>Telephone:</strong>
                        </div>
                        <div className="col-md-6">
                            {patientData.birthDate}
                            <br />
                            {patientData.gender}
                            <br/>
                            {patientData.telecom[0].value}
                        </div>
                    </div>
                </div>
            </div>
            <div className="col-lg-3">
                <address>
                    {patientData.address[0].line[0]}<br />
                    {patientData.address[0].city}, {patientData.address[0].state} {patientData.address[0].postalCode} <br />
                    {patientData.address[0].country}
                </address>
            </div>
        </div>
    );
}

export default PatientDataPanel