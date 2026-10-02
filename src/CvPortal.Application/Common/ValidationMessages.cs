namespace CvPortal.Application.Common;

public static class ValidationMessages
{
    public const string Required = "{0} alanı zorunludur.";
    public const string MaxLength = "{0} en fazla {1} karakter olabilir.";
    public const string Length = "{0} {2} ile {1} karakter arasında olmalıdır.";
    public const string Email = "Geçerli bir e-posta adresi giriniz.";
    public const string Url = "Geçerli bir URL giriniz (https://...).";
    public const string Phone = "Geçerli bir telefon numarası giriniz.";
    public const string Range = "{0} {1} ile {2} arasında olmalıdır.";
}
