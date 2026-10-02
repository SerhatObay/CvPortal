using CvPortal.Application.Abstractions;
using CvPortal.Application.DTOs;
using CvPortal.Application.Mapping;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Entities;

namespace CvPortal.Application.Services;

public sealed class ContactMessageService(IRepository<ContactMessage> repository, TimeProvider time) : IContactMessageService
{
    public async Task<IReadOnlyList<ContactMessageDto>> GetAllAsync(bool onlyUnread = false, CancellationToken ct = default)
    {
        var items = await repository.GetAllAsync(ct);
        return items
            .Where(m => !onlyUnread || !m.IsRead)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.ToDto())
            .ToList();
    }

    public async Task<ContactMessageDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => (await repository.GetByIdAsync(id, ct))?.ToDto();

    public async Task<ContactMessageDto> CreateAsync(CreateContactMessageDto dto, CancellationToken ct = default)
    {
        var message = new ContactMessage
        {
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim(),
            Subject = dto.Subject.Trim(),
            Message = dto.Message.Trim(),
            CreatedAt = time.GetUtcNow().UtcDateTime
        };

        return (await repository.AddAsync(message, ct)).ToDto();
    }

    public async Task<bool> SetReadStatusAsync(int id, bool isRead, CancellationToken ct = default)
    {
        var message = await repository.GetByIdAsync(id, ct);
        if (message is null) return false;
        if (message.IsRead == isRead) return true;

        message.IsRead = isRead;
        message.ReadAt = isRead ? time.GetUtcNow().UtcDateTime : null;
        return await repository.UpdateAsync(message, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default) => repository.DeleteAsync(id, ct);

    public Task<int> GetUnreadCountAsync(CancellationToken ct = default) => repository.CountAsync(m => !m.IsRead, ct);
}
