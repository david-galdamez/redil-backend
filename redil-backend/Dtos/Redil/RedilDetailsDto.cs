namespace redil_backend.Dtos.Redil
{
    public record RedilDetailsDto(int Id, string Name, string Description, IEnumerable<RedilTeacherList> TeacherList);

    public record RedilTeacherList(int Id, string Name, string Email);
}
