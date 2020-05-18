import React from 'react'

const PatientGlucoseSummary = ({ patient, summary, changeDate }) => {
    if (summary === "NoData") {
        return(
            <div className="col-lg-12 col-lg-offset-2">
                <h2>No CGM data is available. The patient might not have shared it yet or has revoked access.</h2>
            </div>
        );
    }

    return(
        <div className="col-lg-12">
            <div className="row border-bottom">
                <div className="col-lg-6">
                    <div className="row">
                        <div className="col-lg-1"><h3><b>From</b></h3></div>
                        <div className="col-lg-3">
                            <input type="date"
                                   value={summary.fromDateTime}
                                   onChange={(event) => changeDate(patient.id, event.target.value, summary.toDateTime)} />
                        </div>
                        <div className="col-lg-1"><h3><b>To</b></h3></div>
                        <div className="col-lg-3">
                            <input type="date" 
                                   value={summary.toDateTime} 
                                   onChange={(event) => changeDate(patient.id, summary.fromDateTime, event.target.value)} />
                        </div>
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col-lg-6">
                    <div className="row">
                        <div className="col-lg-12">
                            <h2>Time-In-Range</h2>
                            <br />
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-lg-1">
                        </div> 
                        <div className="col-lg-9">
                        { displayBarRow(60, summary.veryHigh, "#ff2d2d", "Very High (>250 mg/dL)") }
                        { displayBarRow(70, summary.high, "#ffc12d", "High") }
                        { displayBarRow(140, summary.onTarget, "#03d91d", "On Target (70 - 180 mg/dL)") }
                        { displayBarRow(70, summary.low, "#7b53ff", "Low") }
                        { displayBarRow(50, summary.veryLow, "#3f3bf2", "Very Low (<54 mg/dL)") }
                        </div> 
                    </div>
                </div>
                <div className="col-lg-6">
                    <div className="row">
                        <h2>Statistics</h2>
                        <br />
                    </div>
                    { displayBigTextRow("#days", summary.days) }
                    { displayBigTextRow("Average Glucose", summary.averageGlucose) }
                    { displayBigTextRow("Glucose Management Indicator (GMI)", summary.glucoseManagementIndicator) }
                    <br />
                    <br />
                    <br />
                    <br />
                    <div className="row">
                        <div className="col-lg-12">
                            <div className="row">
                                <div className="col-lg-4">
                                    <b>Glucose Ranges</b>
                                </div>
                                <div className="col-lg-6">
                                    <b>Targets</b> [% of Readings (Time/Day)]
                                </div>
                            </div>
                            {displaySmallTextRow("Target Range 70-180 mg/dL", summary.readingTimeDayOnTarget)}
                            {displaySmallTextRow("Below 70 mg/dL", summary.readingTimeDayLow)}
                            {displaySmallTextRow("Below 54 mg/dL", summary.readingTimeDayVeryLow)}
                            {displaySmallTextRow("Above 250 mg/dL", summary.readingTimeDayVeryHigh)}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
};

const displayBarRow = (height, percentage, color, value) => {
    return (
        <div className="row">
            <div className="col-lg-6" style={{backgroundColor: color, height: height, width: "80px"}}>
                <h3 style={{color: "black"}}>{ percentage }</h3>
            </div>
            <div className="col-lg-6">
                <h3>{ value }</h3>
            </div>
        </div>
    );
}

const displayBigTextRow = (description, value ) => {
    return (
        <div className="row">
            <div className="col-lg-6">
                <h3>{ description }</h3>
            </div>
            <div className="col-lg-6">
                <h3>{ value }</h3>
            </div>
        </div>
    );
}

const displaySmallTextRow = (description, value ) => {
    return (
        <div className="row">
            <div className="col-lg-4">
                { description }
            </div>
            <div className="col-lg-6">
                { value }
            </div>
        </div>
    );
}

export default PatientGlucoseSummary