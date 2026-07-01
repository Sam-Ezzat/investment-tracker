using Microsoft.EntityFrameworkCore;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Infrastructure.Data;

namespace InvestmentTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for Participant operations
/// </summary>
public class ParticipantService : IParticipantService
{
    private readonly ApplicationDbContext _context;

    public ParticipantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ParticipantDto>> GetAllAsync()
    {
        return await _context.Participants
            .Select(p => new ParticipantDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                Phone = p.Phone,
                NationalId = p.NationalId,
                Address = p.Address,
                DateOfBirth = p.DateOfBirth,
                RegistrationDate = p.RegistrationDate,
                IsActive = p.IsActive,
                Notes = p.Notes,
                TotalInvestmentCycles = p.InvestmentCycles.Count,
                TotalInvestedAmount = p.InvestmentCycles.Sum(ic => ic.PrincipalAmount),
                ActiveCyclesCount = p.InvestmentCycles.Count(ic => ic.Status == Core.Enums.InvestmentStatus.Active)
            })
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParticipantDto>> GetActiveAsync()
    {
        return await _context.Participants
            .Where(p => p.IsActive)
            .Select(p => new ParticipantDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                Phone = p.Phone,
                NationalId = p.NationalId,
                Address = p.Address,
                DateOfBirth = p.DateOfBirth,
                RegistrationDate = p.RegistrationDate,
                IsActive = p.IsActive,
                Notes = p.Notes,
                TotalInvestmentCycles = p.InvestmentCycles.Count,
                TotalInvestedAmount = p.InvestmentCycles.Sum(ic => ic.PrincipalAmount),
                ActiveCyclesCount = p.InvestmentCycles.Count(ic => ic.Status == Core.Enums.InvestmentStatus.Active)
            })
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }

    public async Task<ParticipantDto?> GetByIdAsync(int id)
    {
        return await _context.Participants
            .Where(p => p.Id == id)
            .Select(p => new ParticipantDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                Phone = p.Phone,
                NationalId = p.NationalId,
                Address = p.Address,
                DateOfBirth = p.DateOfBirth,
                RegistrationDate = p.RegistrationDate,
                IsActive = p.IsActive,
                Notes = p.Notes,
                TotalInvestmentCycles = p.InvestmentCycles.Count,
                TotalInvestedAmount = p.InvestmentCycles.Sum(ic => ic.PrincipalAmount),
                ActiveCyclesCount = p.InvestmentCycles.Count(ic => ic.Status == Core.Enums.InvestmentStatus.Active)
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ParticipantDto>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        
        return await _context.Participants
            .Where(p => p.FullName.ToLower().Contains(term) ||
                       (p.Email != null && p.Email.ToLower().Contains(term)) ||
                       p.Phone.Contains(term) ||
                       (p.NationalId != null && p.NationalId.Contains(term)))
            .Select(p => new ParticipantDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Email = p.Email,
                Phone = p.Phone,
                NationalId = p.NationalId,
                Address = p.Address,
                DateOfBirth = p.DateOfBirth,
                RegistrationDate = p.RegistrationDate,
                IsActive = p.IsActive,
                Notes = p.Notes,
                TotalInvestmentCycles = p.InvestmentCycles.Count,
                TotalInvestedAmount = p.InvestmentCycles.Sum(ic => ic.PrincipalAmount),
                ActiveCyclesCount = p.InvestmentCycles.Count(ic => ic.Status == Core.Enums.InvestmentStatus.Active)
            })
            .OrderBy(p => p.FullName)
            .ToListAsync();
    }

    public async Task<ParticipantDto> CreateAsync(CreateParticipantDto dto)
    {
        var participant = new Participant
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            NationalId = dto.NationalId,
            Address = dto.Address,
            DateOfBirth = dto.DateOfBirth,
            RegistrationDate = dto.RegistrationDate,
            IsActive = true,
            Notes = dto.Notes
        };

        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(participant.Id))!;
    }

    public async Task<ParticipantDto> UpdateAsync(int id, CreateParticipantDto dto)
    {
        var participant = await _context.Participants.FindAsync(id);
        if (participant == null)
            throw new KeyNotFoundException($"Participant with ID {id} not found.");

        participant.FullName = dto.FullName;
        participant.Email = dto.Email;
        participant.Phone = dto.Phone;
        participant.NationalId = dto.NationalId;
        participant.Address = dto.Address;
        participant.DateOfBirth = dto.DateOfBirth;
        participant.RegistrationDate = dto.RegistrationDate;
        participant.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var participant = await _context.Participants.FindAsync(id);
        if (participant == null)
            return false;

        // Soft delete
        participant.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var query = _context.Participants.Where(p => p.Email == email);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> NationalIdExistsAsync(string nationalId, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return false;

        var query = _context.Participants.Where(p => p.NationalId == nationalId);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}
