using Microsoft.EntityFrameworkCore;
using redil_backend.Dtos;
using redil_backend.Dtos.Classes;
using redil_backend.Dtos.Redil;
using redil_backend.Mappers;
using redil_backend.Models;
using redil_backend.Repository.ClassDetails;
using redil_backend.Repository.Classes;
using redil_backend.Repository.Redil;
using redil_backend.Repository.StudentRediles;
using redil_backend.Repository.Students;
using redil_backend.Utils;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace redil_backend.Services.Classes
{
    public class ClassService : IClassService<ServiceResult<ClassDto>, RegisterClassDto>
    {
        private IClassRepository<Class> _classRepository;
        private IStudentRedilRepository<StudentRedil> _studentRedilRepository;
        private IClassDetailsRepository<ClassDetail> _classDetailsRepository;
        private IStudentRepository<Student> _studentRepository;
        private IRedilRepository<Redile> _redilRepository;

        public ClassService(
            IClassRepository<Class> classRepository,
            IStudentRedilRepository<StudentRedil> studentRedilRepository,
            IClassDetailsRepository<ClassDetail> classDetailsRepository,
            IStudentRepository<Student> studentRepository,
            IRedilRepository<Redile> redilRepository)
        {
            _studentRedilRepository = studentRedilRepository;
            _classRepository = classRepository;
            _classDetailsRepository = classDetailsRepository;
            _studentRepository = studentRepository;
            _redilRepository = redilRepository;
        }

        public async Task<bool> ClassExists(int classId)
        {
            return await _classRepository.Exists(classId);
        }

        public async Task<bool> ClassExists(string attendanceToken)
        {
            return await _classRepository.Exists(attendanceToken);
        }

        public async Task<ServiceResult<AssistStatusDto>> GetAssistStatus(string attendanceToken)
        {
            var validAssist = await ValidateAssistToken(attendanceToken);
            if(!validAssist)
            {
                return ServiceResult<AssistStatusDto>.Fail("Token de asistencia inválido o expirado.");
            }

            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if(classModel == null)
            {
                return ServiceResult<AssistStatusDto>.Fail("Clase no encontrada.");
            }

            var redil = await _redilRepository.GetRedilById(classModel.RedilId);
            if(redil == null)
            {
                return ServiceResult<AssistStatusDto>.Fail("Redil no encontrado.");
            }

            var phones = await _classDetailsRepository.GetPhonesByClassId(classModel.Id);

            var assistStatus = new AssistStatusDto(redil.Name, classModel.ClassDescription, classModel.ClassDate, phones);
            return ServiceResult<AssistStatusDto>.Ok(assistStatus);
        }

        public async Task<ServiceResult<ClassDetailsDto>> GetClassDetail(int classId)
        {
            var classDetail = await _classRepository.GetById(classId);

            if(classDetail == null)
            {
                return ServiceResult<ClassDetailsDto>.Fail("Clase no encontrada.");
            }

            return ServiceResult<ClassDetailsDto>.Ok(classDetail.ToClassDetailsDto());
        }

        public async Task<ServiceResult<PaginatedResponse<ClassListDto>>> GetClasses(int redilId, int page)
        {
            var classes = await _classRepository.GetClasses(redilId, page);

            return ServiceResult<PaginatedResponse<ClassListDto>>.Ok(classes);
        }

        public async Task<ServiceResult<PaginatedResponse<RedilClassStatDto>>> GetRedilStats(
            int? redilId, ClassStatsRequestDto classStatsRequest, int page)
        {
            const int pageSize = 10;
            var result = await _classDetailsRepository.GetClassStatsPaged(
                redilId,
                classStatsRequest.FromDate,
                classStatsRequest.ToDate,
                classStatsRequest.GroupId,
                classStatsRequest.Search,
                page,
                pageSize);

            return ServiceResult<PaginatedResponse<RedilClassStatDto>>.Ok(result);
        }

        public async Task<byte[]?> GetRedilStatsExport(int? redilId, ClassStatsRequestDto classStatsRequest)
        {
            var allStats = await _classDetailsRepository.GetClassStatsAll(
                redilId,
                classStatsRequest.FromDate,
                classStatsRequest.ToDate,
                classStatsRequest.GroupId,
                classStatsRequest.Search);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Estadísticas");

            ws.Cell(1, 1).Value = "Estudiante";
            ws.Cell(1, 2).Value = "Grupo";
            ws.Cell(1, 3).Value = "Redil";
            ws.Cell(1, 4).Value = "Tipo";
            ws.Cell(1, 5).Value = "Asistencia (%)";

            var headerRange = ws.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(0xE5E7EB);

            for (int i = 0; i < allStats.Count; i++)
            {
                var stat = allStats[i];
                var row = i + 2;
                ws.Cell(row, 1).Value = stat.Name;
                ws.Cell(row, 2).Value = stat.GroupName;
                ws.Cell(row, 3).Value = string.Join(", ", stat.Rediles);
                ws.Cell(row, 4).Value = stat.IsServer ? "Servidor" : "Pueblo";
                ws.Cell(row, 5).Value = stat.AttendancePercentage;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<ServiceResult<string>> PassAssist(int classId)
        {
            var classModel = await _classRepository.GetById(classId);
            if(classModel == null)
            {
                return ServiceResult<string>.Fail("Clase no encontrada.");
            }

            // Si se intenta generar otro token, se reutiliza el token existente si no ha expirado
            if(classModel.AttendanceToken != null && classModel.ExpiresAt > DateTime.UtcNow)
            {
                return ServiceResult<string>.Ok(classModel.AttendanceToken);
            }

            bool saved = false;
            while(!saved)
            {
                classModel.AttendanceToken = AttendanceTokenGenerator.Generate();
                classModel.ExpiresAt = DateTime.UtcNow.AddHours(1);

                try
                {
                    await _classRepository.Update(classModel);
                    await _classRepository.Save();
                    saved = true;
                }
                catch (DbUpdateException)
                {
                    // Si hay una colisión de token, generamos uno nuevo y lo intentamos de nuevo
                    continue;
                }
            }

            return ServiceResult<string>.Ok(classModel.AttendanceToken!);
        }

        public async Task<ServiceResult<ClassDto>> RegisterAssist(string attendanceToken, RegisterAttendanceDto registerAttendanceDto)
        {
            var validAssist = await ValidateAssistToken(attendanceToken);
            if(!validAssist)
            {
                return ServiceResult<ClassDto>.Fail("Token de asistencia expirado.");
            }

            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if(classModel == null)
            {
                return ServiceResult<ClassDto>.Fail("Clase no encontrada.");
            }

            var studentModel = await _studentRepository.GetStudentByPhone(registerAttendanceDto.Phone, classModel.RedilId);
            if(studentModel == null)
            {
                return ServiceResult<ClassDto>.Fail("Número de teléfono no registrado en el redil.");
            }

            var classDetail = await _classDetailsRepository.GetClassDetail(classModel.Id, studentModel.Id);
            if(classDetail == null)
            {
                return ServiceResult<ClassDto>.Fail("El estudiante no está inscrito en esta clase.");
            }

            if (classDetail.Attendance)
            {
                return ServiceResult<ClassDto>.Fail("Asistencia ya registrada para este estudiante.");
            }

            classDetail.Attendance = registerAttendanceDto.Attended;

            await _classDetailsRepository.Update(classDetail);
            await _classDetailsRepository.Save();

            return ServiceResult<ClassDto>.Ok(classModel.ToClassDto());
        }

        public async Task<ServiceResult<ClassDto>> RegisterClass(RegisterClassDto registerClassDto, int redilId, int teacherId)
        {
            var classModel = registerClassDto.ToClassModel(redilId, teacherId);

            await _classRepository.Add(classModel);
            await _classRepository.Save();

            var students = await _studentRedilRepository.GetStudents(redilId);
            foreach(var student in students)
            {
                await _classDetailsRepository.Add(new ClassDetail
                {
                    ClassId = classModel.Id,
                    StudentId = student.StudentId,
                    Attendance = false
                });
            }

            await _classDetailsRepository.Save();

            var classDto = classModel.ToClassDto();
            return ServiceResult<ClassDto>.Ok(classDto);
        }

        public async Task<ServiceResult<string>> RegisterManualAssist(string attendanceToken, RegisterAttendanceDto registerAttendanceDto)
        {
            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if (classModel == null)
            {
                return ServiceResult<string>.Fail("Clase no encontrada.");
            }

            var studentModel = await _studentRepository.GetStudentByPhone(registerAttendanceDto.Phone, classModel.RedilId);
            if (studentModel == null)
            {
                return ServiceResult<string>.Fail("Número de teléfono no registrado en el redil.");
            }

            var classDetail = await _classDetailsRepository.GetClassDetail(classModel.Id, studentModel.Id);
            if (classDetail == null)
            {
                return ServiceResult<string>.Fail("El estudiante no está inscrito en esta clase.");
            }

            classDetail.Attendance = registerAttendanceDto.Attended;

            await _classDetailsRepository.Update(classDetail);
            await _classDetailsRepository.Save();

            return ServiceResult<string>.Ok(registerAttendanceDto.Attended ? "Asistencia registrada." : "Inasistencia registrada.");
        }

        public async Task<bool> AssistTokenExists(string attendanceToken)
        {
            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if(classModel == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ValidateAssistToken(string attendanceToken)
        {
            var classModel = await _classRepository.GetByAttendanceToken(attendanceToken);
            if(classModel == null)
            {
                return false;
            }

            return classModel.AttendanceToken != null && classModel.ExpiresAt > DateTime.UtcNow;
        }

        public async Task<byte[]> GetRedilStatsPdfExport(int? redilId, ClassStatsRequestDto classStatsRequest, StatsExportFiltersDto filters)
        {
            var allStats = await _classDetailsRepository.GetClassStatsAll(
                redilId,
                classStatsRequest.FromDate,
                classStatsRequest.ToDate,
                classStatsRequest.GroupId,
                classStatsRequest.Search);

            var salvadorTz = TimeZoneInfo.FindSystemTimeZoneById("America/El_Salvador");
            var exportedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, salvadorTz);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Encabezado
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Text("Estadísticas de Rediles")
                                .Bold().FontSize(18).FontColor(Color.FromHex("#1e3a5f"));

                            row.ConstantItem(155).Column(right =>
                            {
                                right.Item().AlignRight()
                                    .Text("Exportado el:").FontSize(8).FontColor(Color.FromHex("#6b7280"));
                                right.Item().AlignRight()
                                    .Text(exportedAt.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Color.FromHex("#6b7280"));
                            });
                        });

                        col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(Color.FromHex("#1e3a5f"));
                        col.Item().Height(10);
                    });

                    // Contenido
                    page.Content().Column(col =>
                    {
                        // Caja de filtros
                        col.Item()
                            .Border(1).BorderColor(Color.FromHex("#d1d5db"))
                            .Background(Color.FromHex("#f9fafb"))
                            .Padding(10)
                            .Column(filterCol =>
                            {
                                filterCol.Item()
                                    .PaddingBottom(5)
                                    .Text("Filtros aplicados")
                                    .Bold().FontSize(10).FontColor(Color.FromHex("#1e3a5f"));

                                filterCol.Item().Text(t =>
                                {
                                    t.Span("Período: ").Bold();
                                    t.Span($"{filters.FromDate:dd/MM/yyyy} – {filters.ToDate:dd/MM/yyyy}");
                                });

                                filterCol.Item().Text(t =>
                                {
                                    t.Span("Redil: ").Bold();
                                    t.Span(filters.RedilName ?? "Todos");
                                });

                                filterCol.Item().Text(t =>
                                {
                                    t.Span("Grupo: ").Bold();
                                    t.Span(filters.GroupName ?? "Todos");
                                });

                                if (!string.IsNullOrWhiteSpace(filters.Search))
                                {
                                    filterCol.Item().Text(t =>
                                    {
                                        t.Span("Búsqueda: ").Bold();
                                        t.Span($"\"{filters.Search}\"");
                                    });
                                }
                            });

                        col.Item().Height(14);

                        // Tabla
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);    // Estudiante
                                cols.RelativeColumn(2);    // Grupo
                                cols.RelativeColumn(2.5f); // Redil(es)
                                cols.RelativeColumn(1.2f); // Tipo
                                cols.RelativeColumn(1.5f); // Asistencia
                            });

                            table.Header(header =>
                            {
                                IContainer HeaderCell(IContainer c) =>
                                    c.Background(Color.FromHex("#1e3a5f")).Padding(6).AlignMiddle();

                                header.Cell().Element(HeaderCell).Text("Estudiante").Bold().FontColor(Colors.White).FontSize(9);
                                header.Cell().Element(HeaderCell).Text("Grupo").Bold().FontColor(Colors.White).FontSize(9);
                                header.Cell().Element(HeaderCell).Text("Redil(es)").Bold().FontColor(Colors.White).FontSize(9);
                                header.Cell().Element(HeaderCell).Text("Tipo").Bold().FontColor(Colors.White).FontSize(9);
                                header.Cell().Element(HeaderCell).AlignRight().Text("Asistencia (%)").Bold().FontColor(Colors.White).FontSize(9);
                            });

                            var rowIndex = 0;
                            foreach (var stat in allStats)
                            {
                                var bg = Color.FromHex(rowIndex++ % 2 == 0 ? "#ffffff" : "#f3f4f6");

                                table.Cell().Background(bg).Padding(5).Text(stat.Name).FontSize(9);
                                table.Cell().Background(bg).Padding(5).Text(stat.GroupName).FontSize(9);
                                table.Cell().Background(bg).Padding(5).Text(string.Join(", ", stat.Rediles)).FontSize(9);
                                table.Cell().Background(bg).Padding(5).Text(stat.IsServer ? "Servidor" : "Pueblo").FontSize(9);
                                table.Cell().Background(bg).Padding(5).AlignRight().Text($"{stat.AttendancePercentage}%").FontSize(9);
                            }
                        });
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ").FontSize(8).FontColor(Color.FromHex("#6b7280"));
                        x.CurrentPageNumber().FontSize(8).FontColor(Color.FromHex("#6b7280"));
                        x.Span(" de ").FontSize(8).FontColor(Color.FromHex("#6b7280"));
                        x.TotalPages().FontSize(8).FontColor(Color.FromHex("#6b7280"));
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
