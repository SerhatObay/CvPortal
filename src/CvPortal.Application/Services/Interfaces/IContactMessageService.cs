using CvPortal.Application.DTOs;

namespace CvPortal.Application.Services.Interfaces;

public interface IContactMessageService
{
    Task<IReadOnlyList<ContactMessageDto>> GetAllAsync(bool onlyUnread = false, CancellationToken ct = default);
    Task<ContactMessageDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ContactMessageDto> CreateAsync(CreateContactMessageDto dto, CancellationToken ct = default);
    Task<bool> SetReadStatusAsync(int id, bool isRead, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(CancellationToken ct = default);
}
