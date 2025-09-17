using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower(); // ép thường
                string[] extensions = { ".jpg", ".png", ".jpeg", ".gif" };   // thêm dấu chấm

                bool result = extensions.Contains(extension);
                if (!result)
                {
                    return new ValidationResult("Allowed extensions are: .jpg, .png, .jpeg, .gif");
                }
            }
            return ValidationResult.Success;
        }
    }
}
