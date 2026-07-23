namespace redil_backend.Dtos.Classes
{
            public record ClassDetailsDto(int ClassId, string ClassDescription, DateTime ClassDate, string? AttendanceToken, bool Expired);
}
