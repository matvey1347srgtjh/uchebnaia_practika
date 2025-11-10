using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CinemaApp.Models;

public class Session
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Выберите фильм")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите фильм")]
    [Display(Name = "Фильм")]
    public int MovieId { get; set; }
    
    [Required(ErrorMessage = "Выберите зал")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите зал")]
    [Display(Name = "Зал")]
    public int HallId { get; set; }
    
    [Required(ErrorMessage = "Укажите дату и время сеанса")]
    [Display(Name = "Дата и время")]
    public DateTime DateTime { get; set; }
    
    [Required(ErrorMessage = "Укажите цену")]
    [Range(0.01, 10000, ErrorMessage = "Цена должна быть от 0.01 до 10000")]
    [Display(Name = "Цена")]
    public decimal Price { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [ValidateNever]
    public virtual Movie? Movie { get; set; }
    
    [ValidateNever]
    public virtual Hall? Hall { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

