
using System;
using System.ComponentModel.DataAnnotations;

public class WeatherRequest
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; }

    [Required]
    [MaxLength(100)]
    public string Country { get; set; }
}