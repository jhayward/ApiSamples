using System;

/// <summary>
/// Product model - only checks required, not all validation rules!
/// </summary>
public class Product
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Product"/> class.
    /// </summary>
    /// <param name="externalId">Your unique id for this product</param>
    /// <param name="name">Name or short description</param>
    /// <param name="price">Price excluding gst</param>
    public Product(string externalId, string sku, string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(externalId))
        {
            throw new ArgumentNullException(nameof(externalId));
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentNullException(nameof(sku));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), $"{nameof(price)} cannot be less than zero");
        }

        ExternalId = externalId;
        Sku = sku;
        Name = name;
        Price = price;
    }

    /// <summary>Your unique Identifier (max len 50)</summary>
    public string ExternalId { get; private set; }

    /// <summary>Your unique model code or SKU code (max len 25)</summary>
    public string Sku { get; set; }

    /// <summary>Name or short description (max len 120)</summary>
    public string Name { get; set; }

    /// <summary>Full long description (max len 1000)</summary>
    public string Description { get; set; }

    /// <summary>Price excluding GST</summary>
    public decimal Price { get; set; }

    /// <summary>Show listing</summary>
    public bool Published { get; set; }

    /// <summary>Amount of available inventory for sale</summary>
    public int Stock { get; set; }

    /// <summary>Barcode number (max len 20)</summary>
    public string Barcode { get; set; }

    /// <summary>weight in kg</summary>
    public decimal? Weight { get; set; }

    /// <summary>height in cm</summary>
    public decimal? Height { get; set; }

    /// <summary>length in cm</summary>
    public decimal? Length { get; set; }

    /// <summary>width in cm</summary>
    public decimal? Width { get; set; }

    /// <summary>Manufacturer Model Number (max len 25)</summary>
    public string ManufacturerSku { get; set; }

    /// <summary>manufacturer Name (max len 50)</summary>
    public string ManufacturerName { get; set; }
}