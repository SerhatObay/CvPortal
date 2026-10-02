using System.ComponentModel.DataAnnotations;

namespace CvPortal.Domain.Enums;

public enum SkillCategory
{
    [Display(Name = "Backend")] Backend = 1,
    [Display(Name = "Frontend")] Frontend = 2,
    [Display(Name = "Veritabanı")] Database = 3,
    [Display(Name = "DevOps & Cloud")] DevOps = 4,
    [Display(Name = "Mimari & Pratikler")] Architecture = 5,
    [Display(Name = "Araçlar")] Tools = 6,
    [Display(Name = "Kişisel Yetkinlik")] SoftSkill = 7
}

public enum EmploymentType
{
    [Display(Name = "Tam Zamanlı")] FullTime = 1,
    [Display(Name = "Yarı Zamanlı")] PartTime = 2,
    [Display(Name = "Sözleşmeli")] Contract = 3,
    [Display(Name = "Freelance")] Freelance = 4,
    [Display(Name = "Stajyer")] Internship = 5
}

public enum WorkMode
{
    [Display(Name = "Ofis")] OnSite = 1,
    [Display(Name = "Uzaktan")] Remote = 2,
    [Display(Name = "Hibrit")] Hybrid = 3
}

public enum LanguageLevel
{
    [Display(Name = "A1 - Başlangıç")] A1 = 1,
    [Display(Name = "A2 - Temel")] A2 = 2,
    [Display(Name = "B1 - Orta")] B1 = 3,
    [Display(Name = "B2 - Orta Üstü")] B2 = 4,
    [Display(Name = "C1 - İleri")] C1 = 5,
    [Display(Name = "C2 - Uzman")] C2 = 6,
    [Display(Name = "Anadil")] Native = 7
}
