using PTTDigital.Authentication.Data.Repository;

namespace PTTDigital.Email.Application.Repositories;

public class Generator : IGenerator
{
    public string GenerateUlid()
    {
        return Ulid.NewUlid().ToString();
    }
}