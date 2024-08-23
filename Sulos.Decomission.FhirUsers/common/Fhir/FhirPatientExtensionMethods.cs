using Hl7.Fhir.Model;
using Sulos.Hospice.Care.Models.Common;
using Sulos.Hospice.Care.Models.Users;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirPatientExtensionMethods
{
    public static SulosPersonModel ToSulosPersonModel(this Patient patient) =>
      new(
          patient.Id,
          patient.GetFirstName(),
          patient.GetLastName(),
          PersonType.Patient
      );

    public static string GetFirstName(this Patient patient) => patient.GetUsualName().GetFirstName();

    public static string GetLastName(this Patient patient) => patient.GetUsualName().GetLastName();

    public static HumanName.NameUse? GetNameUse(this Patient patient) =>
        patient.Name.FirstOrDefault()?.Use;

    private static HumanName? GetUsualName(this Patient patient) =>
    patient.Name.Find(n => n.Use == HumanName.NameUse.Usual);
}