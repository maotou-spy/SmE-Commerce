﻿using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Product;

public class UpdateProductReqDto
{
    [Required(ErrorMessage = "Product name is required")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Product image is required")]
    public required string PrimaryImage { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "Product price is required")]
    [RegularExpression(@"^\d+\.\d{0,2}$", ErrorMessage = "Invalid price")]
    public decimal Price { get; set; }

    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid stock quantity")]
    public int StockQuantity { get; set; } = 0;

    public bool IsTopSeller { get; set; } = false;

    public string Status { get; set; } = ProductStatus.Active;

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }
}
