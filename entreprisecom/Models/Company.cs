namespace entreprisecom.Models;

public sealed record Company(
    string Siren,
    DateTime? CreationDate,
    string? FirstName,
    string WorkforceBand,
    string AdministrativeStatus,
    string? LastName,
    string? Denomination,
    string? ActivityCode);
