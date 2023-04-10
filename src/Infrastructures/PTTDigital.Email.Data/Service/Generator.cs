namespace PTTDigital.Email.Data.Service;

public class Generator : IGenerator
{
    public string GenerateUlid()
    {
        return Ulid.NewUlid().ToString();
    }
}