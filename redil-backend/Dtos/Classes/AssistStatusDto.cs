namespace redil_backend.Dtos.Classes
{
    public record AssistStatusDto(string RedilName, string ClassDescription, DateTime ClassDate, IEnumerable<string> Phones);
}
