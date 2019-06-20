using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApi.Controllers
{
    [Route("api/[controller]")]
    public class UserLogin : Controller
    {
        public AuthenticationService.AuthPortClient authPortClient = new AuthenticationService.AuthPortClient();
        public HttpClient httpClient = new HttpClient();
       
        // GET api/<controller>/logout/afgenjxkoe
        [Route("logout/{token}")]
        public string Get(string token) // user logout
        {
            try
            {
                AuthenticationService.logoutRequest logoutRequest = new AuthenticationService.logoutRequest();
                logoutRequest.token = token;
                Task<AuthenticationService.logoutResponse1> logoutResponse1 = this.authPortClient.logoutAsync(logoutRequest);
                return "{'status': '" + logoutResponse1.Result.logoutResponse.status + "'}";
            }
            catch (Exception e)
            {
                return "{'status': 'failed'}";
            }
        }

        // POST api/<controller>
        [HttpPost]
        public string Post([FromBody] AuthenticationService.loginRequest loginRequest) // user login
        {
            try
            {
                Task<AuthenticationService.loginResponse1> loginResponse1 = this.authPortClient.loginAsync(loginRequest);
                patientApi.Client client = new patientApi.Client(this.httpClient);
                Task<patientApi.Patient> patient = client.GetPatientAsync(loginResponse1.Result.loginResponse.token, loginResponse1.Result.loginResponse.userId);
                string healthPro = "";
                foreach(int healthProId in patient.Result.HealthProfessionalIds)
                {
                    healthPro = healthPro + healthProId.ToString() + ",";
                }
                healthPro = healthPro.Length > 0 ? healthPro.Substring(0, healthPro.Length - 1) : healthPro;
                return ("{'token': '" + loginResponse1.Result.loginResponse.token +
                        "', 'userId': '" + loginResponse1.Result.loginResponse.userId.ToString() +
                        "', 'username': '" + patient.Result.Username +
                        "', 'firstname': '" + patient.Result.FirstName +
                        "', 'lastname': '" + patient.Result.LastName +
                        "', 'dateOfBirth': '" + patient.Result.DateOfBirth +
                        "', 'address': '" + patient.Result.Address +
                        "', 'contactNumber': '" + patient.Result.ContactNumber +
                        "', 'emailAddress': '" + patient.Result.EmailAddress +
                        "', 'healthProIds': '" + healthPro +
                        "'}"
                       );
            }
            catch (Exception e)
            {
                return "{'status': 'failed'}";
            }
        }

        // PUT api/<controller>
        [HttpPut]
        public string Put([FromBody] NewPatient newPatient)
        {
            try
            {
                patientApi.Client client = new patientApi.Client(this.httpClient);
                Task<patientApi.Patient> patient = client.PutPatientAsync(newPatient.token, newPatient, newPatient.PatientId ?? -200);
                if (patient.Result.PatientId == newPatient.PatientId)
                {
                    string healthPro = "";
                    foreach (int healthProId in patient.Result.HealthProfessionalIds)
                    {
                        healthPro = healthPro + healthProId.ToString() + ",";
                    }
                    healthPro = healthPro.Length > 0 ? healthPro.Substring(0, healthPro.Length - 1) : healthPro;
                    return ("{'token': '" + newPatient.token +
                            "', 'userId': '" + newPatient.PatientId.ToString() +
                            "', 'username': '" + newPatient.Username +
                            "', 'firstname': '" + patient.Result.FirstName +
                            "', 'lastname': '" + patient.Result.LastName +
                            "', 'dateOfBirth': '" + patient.Result.DateOfBirth +
                            "', 'address': '" + patient.Result.Address +
                            "', 'contactNumber': '" + patient.Result.ContactNumber +
                            "', 'emailAddress': '" + patient.Result.EmailAddress +
                            "', 'healthProIds': '" + healthPro +
                            "'}"
                           );
                }
                else
                {
                    return "{'status': 'failed'}";
                }
            } catch(Exception e)
            {
                return "{'status':"+ e.Message +"}";
            }

        }

        // POST api/<controller>/healthpro
        [Route("healthpro")]

        public string Post([FromBody] HealthPro healthPro) // Getting healthPro data
        {
            try
            {
                HealthProApi.Client client = new HealthProApi.Client(httpClient);
                Task<ICollection<HealthProApi.HealthProfessional>> healthProData = client.GetHealthProfessionalsAsync(healthPro.token, healthPro.healthProIds);
                string result = "";
                foreach(HealthProApi.HealthProfessional data in healthProData.Result )
                {
                    result = result + "{'firstname': '" + data.FirstName +
                            "', 'lastname': '" + data.LastName +
                            "', 'contactNumber': '" + data.ContactNumber +
                            "'}, ";
                }
                result = result.Length > 0 ? result.Substring(0, result.Length - 2) : result;
                return ("[" + result + "]");
            } 
            catch (Exception e)
            {
                return "{'status': 'failed'}";
            }
        }
    }

    public class HealthPro
    {
        public string healthProIds { get; set; }
        public string token { get; set; }
    }

    public class NewPatient: patientApi.Patient
    {
        public string token { get; set; }
    }
}
