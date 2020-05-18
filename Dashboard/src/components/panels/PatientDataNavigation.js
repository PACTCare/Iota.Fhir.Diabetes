import React from 'react'

const PatientDataNavigation = ({ patientData, selectMenu }) => {
    return(
        <div className="col-lg-12">
        <div className="col-lg-2">
            <div className="col-lg-6 col-lg-offset-1" style={{textAlign: "center"}}>
                <button className="btn btn-default">
                    <i className="fa fa-share-square-o fa-2x"></i>
                </button>
                <h5>Send to EPD</h5>
            </div>
        </div>
        <div className="col-lg-4"></div>
        <div className="col-lg-6">
            <button className="btn btn-w-m btn-default btn-lg" onClick={() => selectMenu("Summary")}>Summary</button>
            &nbsp;
            &nbsp;
            <button className="btn btn-w-m btn-default btn-lg" onClick={() => selectMenu("Daily")}>Daily</button>
            &nbsp;
            &nbsp;
            <button className="btn btn-w-m btn-default btn-lg" onClick={() => selectMenu("Clinical")}>Clinical</button>
            &nbsp;
            &nbsp;
            <button className="btn btn-w-m btn-default btn-lg" onClick={() => selectMenu("Reported")}>Reported</button>
        </div>
    </div>

    )
};

export default PatientDataNavigation