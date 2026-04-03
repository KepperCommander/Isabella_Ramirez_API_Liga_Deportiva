using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Text.RegularExpressions;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
        ISponsorRepository sponsorRepository,
        ITournamentSponsorRepository tournamentSponsorRepository,
        ITournamentRepository tournamentRepository,
        ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _tournamentRepository = tournamentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync() => await _sponsorRepository.GetAllAsync();

    public async Task<Sponsor?> GetByIdAsync(int id) => await _sponsorRepository.GetByIdAsync(id);

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        await ValidateBusinessRulesAsync(sponsor); // 
        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Sponsor con ID {id} no existe.");

        if (existing.Name != sponsor.Name) await ValidateBusinessRulesAsync(sponsor);

        existing.Name = sponsor.Name;
        existing.ContactEmail = sponsor.ContactEmail;
        existing.Phone = sponsor.Phone;
        existing.WebsiteUrl = sponsor.WebsiteUrl;
        existing.Category = sponsor.Category;

        await _sponsorRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _sponsorRepository.ExistsAsync(id)) throw new KeyNotFoundException($"Sponsor {id} no encontrado.");
        await _sponsorRepository.DeleteAsync(id);
    }

    public async Task RegisterSponsorToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
    {
        if (contractAmount <= 0)
            throw new InvalidOperationException("ContractAmount debe ser mayor a 0."); // 

        if (!await _sponsorRepository.ExistsAsync(sponsorId))
            throw new KeyNotFoundException($"Sponsor {sponsorId} no existe."); // 

        if (!await _tournamentRepository.ExistsAsync(tournamentId))
            throw new KeyNotFoundException($"Tournament {tournamentId} no existe."); // 

        var existingLink = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existingLink != null)
            throw new InvalidOperationException("Vinculación duplicada: el sponsor ya está en este torneo."); // 

        await _tournamentSponsorRepository.CreateAsync(new TournamentSponsor
        {
            SponsorId = sponsorId,
            TournamentId = tournamentId,
            ContractAmount = contractAmount
        });
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        if (!await _sponsorRepository.ExistsAsync(sponsorId))
            throw new KeyNotFoundException($"Sponsor {sponsorId} no encontrado.");

        var links = await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
        return links.Select(ts => ts.Tournament);
    }

    public async Task RemoveSponsorFromTournamentAsync(int sponsorId, int tournamentId)
    {
        var link = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (link == null) throw new KeyNotFoundException("Vinculación no encontrada.");

        await _tournamentSponsorRepository.DeleteAsync(link.Id);
    }

    private async Task ValidateBusinessRulesAsync(Sponsor sponsor)
    {
        if (!Regex.IsMatch(sponsor.ContactEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new InvalidOperationException("ContactEmail debe ser un formato válido."); // 

        var existing = await _sponsorRepository.GetByNameAsync(sponsor.Name);
        if (existing != null)
            throw new InvalidOperationException($"No se puede crear: Name '{sponsor.Name}' duplicado."); // 
    }
}