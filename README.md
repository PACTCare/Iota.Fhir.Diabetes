This repository represents how to create a web app (API and UI) plus a Mobile App, to work with IOTA and FHIR. It is part of the Untangle Care EDF grant, that has been concluded on May the 15th 2020.

While the approach used here can be applied to all FHIR resources, this demostration usecase evolves around diabetes glucose exchange along with clinical resources.

The steps included, can be described as follows (technical explanations left out on purpose, see https://florencedigitalhealth.atlassian.net/wiki/spaces/IF/pages/64126981/Introduction+and+Goals and https://github.com/PACTCare/Pact.Fhir for more details):

- The first step is the data extraction from the blood glucose sensor. In this example, the data is retrieved from the Dexcom API. The Dexcom API is RESTful and utilizes the OAuth 2.0 standard for authentication. This sensor generates one BG measurement every 10 minutes
- After the raw data is read via the API, it is converted into a FHIR JSON resource. This conversation is done by filling an FHIR observation blueprint for each generated glucose data measurement with the date of when the glucose measurement is generated and the value of this measurement. The output of this module is one FHIR JSON file per each BG measurement.
These files are stored locally on the device and the patient can access it from a “patient interface” (similar to a personal health record, dedicated to diabetes)
When the patient provides consent, the FHIR JSON resources receive an ID and are uploaded into a decentralized storage solution (IOTA/the Tangle) that is used to enable the sharing of this data into the physician dashboard
- Receiving the glucose data can finally be done by reading the data stream shared by the patient and by parsing the glucose JSON resources into the doctor’s dashboard
This dashboard provides an overview of the patients diabetes management status and enables exporting of this data (using the FHIR format) into any compatible Electronic Medical Record system (via a FHIR API)
