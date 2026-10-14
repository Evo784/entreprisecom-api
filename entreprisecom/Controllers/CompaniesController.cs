using entreprisecom.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace entreprisecom.Controllers;

[ApiController]
[Route("api/entreprises")]
public sealed class CompaniesController(MySqlDataSource dataSource) : ControllerBase
{
    [HttpGet("{siren}")]
    public async Task<ActionResult<Company>> GetBySiren(string siren)
    {
        if (!TryParseSiren(siren, out var id))
            return BadRequest("Le SIREN doit contenir 9 chiffres.");

        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new MySqlCommand("SELECT * FROM mytable WHERE siren = @siren", connection);
        command.Parameters.AddWithValue("@siren", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return NotFound();
        return Ok(ReadCompany(reader));
    }

    [HttpGet]
    public async Task<ActionResult<List<Company>>> Search(
        [FromQuery] string? codeActivite,
        [FromQuery] string? denomination,
        [FromQuery] int page = 1)
    {
        var byActivity = !string.IsNullOrWhiteSpace(codeActivite);
        var byName = !string.IsNullOrWhiteSpace(denomination);
        if (byActivity == byName)
            return BadRequest("Indique soit codeActivite, soit denomination.");
        if (page < 1)
            return BadRequest("La page doit être supérieure ou égale à 1.");

        const int pageSize = 100;
        long offset = ((long)page - 1) * pageSize;
        var sql = byActivity
            ? "SELECT * FROM mytable WHERE activite_principale_unite_legale = @value ORDER BY siren LIMIT 100 OFFSET @offset"
            : "SELECT * FROM mytable WHERE LOCATE(LOWER(@value), LOWER(denomination_unite_legale)) > 0 ORDER BY siren LIMIT 100 OFFSET @offset";

        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@value", byActivity ? codeActivite!.Trim() : denomination!.Trim());
        command.Parameters.AddWithValue("@offset", offset);

        await using var reader = await command.ExecuteReaderAsync();
        var companies = new List<Company>();
        while (await reader.ReadAsync()) companies.Add(ReadCompany(reader));
        return Ok(companies);
    }

    [HttpDelete("{siren}")]
    public async Task<IActionResult> Delete(string siren)
    {
        if (!TryParseSiren(siren, out var id))
            return BadRequest("Le SIREN doit contenir 9 chiffres.");

        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = new MySqlCommand("DELETE FROM mytable WHERE siren = @siren", connection);
        command.Parameters.AddWithValue("@siren", id);

        return await command.ExecuteNonQueryAsync() == 0 ? NotFound() : NoContent();
    }

    private static bool TryParseSiren(string siren, out int id)
    {
        id = 0;
        return siren.Length == 9 && siren.All(char.IsAsciiDigit) && int.TryParse(siren, out id);
    }

    private static Company ReadCompany(MySqlDataReader reader) => new(
        reader.GetInt32("siren").ToString("D9"),
        reader.IsDBNull(reader.GetOrdinal("date_creation_unite_legale")) ? null : reader.GetDateTime("date_creation_unite_legale"),
        reader.IsDBNull(reader.GetOrdinal("prenom_unite_legale")) ? null : reader.GetString("prenom_unite_legale"),
        reader.GetString("tranche_effectifs_unite_legale"),
        reader.GetString("etat_administratif_unite_legale"),
        reader.IsDBNull(reader.GetOrdinal("nom_unite_legale")) ? null : reader.GetString("nom_unite_legale"),
        reader.IsDBNull(reader.GetOrdinal("denomination_unite_legale")) ? null : reader.GetString("denomination_unite_legale"),
        reader.IsDBNull(reader.GetOrdinal("activite_principale_unite_legale")) ? null : reader.GetString("activite_principale_unite_legale"));
}
