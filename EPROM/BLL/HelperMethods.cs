using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class HelperMethods
    {
        public static Entities GetEntities()
        {
            return new Entities();
        }

        public static List<Patient_model> ReadCSVFile(Stream streamfile, string ProviderId, string OrganizationId, string PracticeId)
        {
            var csvReader = new StreamReader(streamfile);
            var csvData = new List<CsvPatient>();
            var patientData = new List<Patient_model>();

            while (!csvReader.EndOfStream)
            {
                var line = csvReader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                var x = line.Split(',').ToList();
                if (string.Equals(x[0], "first name", StringComparison.OrdinalIgnoreCase)) continue;
                if (x.Count < 16) continue;
                csvData.Add(new CsvPatient
                {
                    FirstName = x[0],
                    MiddleName = x[1],
                    LastName = x[2],
                    Line1 = x[3],
                    Line2 = x[4],
                    DOB = x[5],
                    Gender = x[6],
                    Suburb = x[7],
                    PostalCode = x[8],
                    State = x[9],
                    Country = x[10],
                    PrimaryPhone = x[11],
                    Email = x[12],
                    MedicareNumber = x[13],
                    IHINumber = x[14],
                    Salutation = x[15]
                });
            }
            for (int i = 0; i < csvData.Count(); i++)
            {
                var patient = Patients.SearchPatientDetail(csvData[i].IHINumber, csvData[i].MedicareNumber, new Guid(ProviderId), new Guid(OrganizationId), new Guid(PracticeId));

                if (patient != null)
                {                    
                    patientData.Add(patient);
                }
                else
                {
                    var patientDetail = new Patient_model
                    {
                        Salutation = csvData[i].Salutation,
                        IHINumber = csvData[i].IHINumber,
                        MedicareNumber = csvData[i].MedicareNumber,

                        User = new User_model { FirstName = csvData[i].FirstName, MiddleName = csvData[i].MiddleName, LastName = csvData[i].LastName, PhoneNumber = csvData[i].PrimaryPhone, DOB = Convert.ToDateTime(csvData[i].DOB), Gender = csvData[i].Gender, Email = csvData[i].Email },

                        Address = new Address_model { Suburb = csvData[i].Suburb, ZipCode = csvData[i].PostalCode, State = csvData[i].State, Country = csvData[i].Country, Line1 = csvData[i].Line1, Line2 = csvData[i].Line2 },

                        Contact = new Contact_model { Phone = csvData[i].PrimaryPhone },
                        IsPatientExist = false
                    };

                    patientData.Add(patientDetail);
                }
            }
            return patientData;
        }
    }
}
