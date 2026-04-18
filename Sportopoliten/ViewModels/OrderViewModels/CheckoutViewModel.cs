using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.ViewModels.OrderViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Введите ваше имя")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите город")]
        public string City { get; set; }

        [Required(ErrorMessage = "Введите адрес")]
        public string Address { get; set; }

        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Выберите страну")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Выберите способ доставки")]
        public string DeliveryMethod { get; set; }

        [Required(ErrorMessage = "Выберите способ оплаты")]
        public string PaymentMethod { get; set; }
        public string? Comment { get; set; }
    }
}