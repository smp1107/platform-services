namespace WebApplication1.Services.Domain.Model.ValueObjects;

public class RentalOrderAddress
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }

    // Constructor para EF Core
    public RentalOrderAddress()
    {
        Street = string.Empty;
        City = string.Empty;
        PostalCode = string.Empty;
    }

    public RentalOrderAddress(string street, string city, string postalCode)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
    }
}