namespace CvPortal.Application.Common;

/// <summary>
/// Kimliği olan DTO'lar için ortak sözleşme (generic controller/servisler için).
/// </summary>
public interface IHasId
{
    int Id { get; set; }
}
