using System.ComponentModel.DataAnnotations;

namespace OperationPrime.Domain.Enums;

/// <summary>
/// Predefined values for the number of users impacted by an incident.
/// These values are used in dropdown selection for consistent incident reporting.
/// </summary>
public enum ImpactedUsersCount
{
    /// <summary>
    /// Approximately 5 users impacted
    /// </summary>
    [Display(Name = "5~")]
    Five = 5,

    /// <summary>
    /// Approximately 10 users impacted
    /// </summary>
    [Display(Name = "10~")]
    Ten = 10,

    /// <summary>
    /// Approximately 20 users impacted
    /// </summary>
    [Display(Name = "20~")]
    Twenty = 20,

    /// <summary>
    /// Approximately 30 users impacted
    /// </summary>
    [Display(Name = "30~")]
    Thirty = 30,

    /// <summary>
    /// Approximately 40 users impacted
    /// </summary>
    [Display(Name = "40~")]
    Forty = 40,

    /// <summary>
    /// Approximately 50 users impacted
    /// </summary>
    [Display(Name = "50~")]
    Fifty = 50,

    /// <summary>
    /// Approximately 60 users impacted
    /// </summary>
    [Display(Name = "60~")]
    Sixty = 60,

    /// <summary>
    /// Approximately 70 users impacted
    /// </summary>
    [Display(Name = "70~")]
    Seventy = 70,

    /// <summary>
    /// Approximately 80 users impacted
    /// </summary>
    [Display(Name = "80~")]
    Eighty = 80,

    /// <summary>
    /// Approximately 90 users impacted
    /// </summary>
    [Display(Name = "90~")]
    Ninety = 90,

    /// <summary>
    /// Approximately 100 users impacted
    /// </summary>
    [Display(Name = "100~")]
    OneHundred = 100,

    /// <summary>
    /// Approximately 200 users impacted
    /// </summary>
    [Display(Name = "200~")]
    TwoHundred = 200,

    /// <summary>
    /// Approximately 300 users impacted
    /// </summary>
    [Display(Name = "300~")]
    ThreeHundred = 300,

    /// <summary>
    /// Approximately 500 users impacted
    /// </summary>
    [Display(Name = "500~")]
    FiveHundred = 500,

    /// <summary>
    /// Approximately 600 users impacted
    /// </summary>
    [Display(Name = "600~")]
    SixHundred = 600,

    /// <summary>
    /// Approximately 800 users impacted
    /// </summary>
    [Display(Name = "800~")]
    EightHundred = 800,

    /// <summary>
    /// Approximately 1000 users impacted
    /// </summary>
    [Display(Name = "1000~")]
    OneThousand = 1000,

    /// <summary>
    /// Approximately 2000 users impacted
    /// </summary>
    [Display(Name = "2000~")]
    TwoThousand = 2000,

    /// <summary>
    /// Approximately 5000 users impacted
    /// </summary>
    [Display(Name = "5000~")]
    FiveThousand = 5000
}
