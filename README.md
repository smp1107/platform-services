# Notas - Desarrollo Web con .NET

## Estructura general del proyecto

El proyecto siempre tiene dos partes: Shared (no se toca nunca) y el Bounded Context (BC) que cambia según el caso.

La carpeta Shared contiene: Result.cs, IAuditableEntity.cs, Error.cs, IBaseRepository.cs, IUnitOfWork.cs, KebabCaseRouteNamingConvention.cs, AppDbContext.cs, AuditableEntityInterceptor.cs, BaseRepository.cs, UnitOfWork.cs, ProblemDetailsFactory.cs, CommonMessages.cs y ErrorMessage.cs.

---

## Orden de creación de archivos

1. Enums simples (ValueObjects tipo enum)
2. Records de owned attributes (ValueObjects compuestos)
3. Enum de errores del BC
4. Command (record con los parámetros de entrada)
5. Aggregate Root (clase principal con IAuditableEntity)
6. Interface del repositorio
7. Interface del command service
8. Implementación del command service
9. Implementación del repositorio
10. ModelBuilderExtensions
11. Resources (request y response)
12. Los tres assemblers
13. Controller
14. Program.cs y appsettings.json
15. Migración y prueba

---

## ValueObjects

Enum simple - asignar números si el enunciado los define:
public enum ENombre { Valor1 = 1, Valor2 = 2, Valor3 = 3 }

Owned attribute - siempre con constructor vacío para EF Core:
public record NombreVO(string Campo1, string Campo2, string Campo3)
{ public NombreVO() : this(string.Empty, string.Empty, string.Empty) { } }

Identificarlos: "owned attribute" en el enunciado = VO obligatorio. Enum con valores definidos = VO obligatorio.

---

## Enum de errores

public enum NombreBCErrors { None, DuplicadoX, InvalidoY, OperationCancelled, DatabaseError, InternalServerError }

Agregar un valor por cada regla de negocio del enunciado. InternalServerError siempre al final.

---

## Command

public record CreateNombreCommand(string Campo1, ENombre Campo2, double Campo3);

No incluir: Id, CreatedAt, UpdatedAt, ni campos que calcula el sistema.

---

## Aggregate Root

Propiedades con private set. CreatedAt y UpdatedAt con public set (los llena el interceptor). Constructor protected vacío para EF Core. Constructor público que recibe el Command.

public class Nombre : IAuditableEntity
{
    public int Id { get; private set; }
    public string Campo { get; private set; }
    public ENombre EnumCampo { get; private set; }
    public NombreVO VoCampo { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    protected Nombre() { Campo = string.Empty; VoCampo = new NombreVO(); }
    public Nombre(CreateNombreCommand command)
    { Campo = command.Campo; EnumCampo = command.EnumCampo; VoCampo = new NombreVO(command.C1, command.C2, command.C3); }
}

---

## Repositorio - Interface

public interface INombreRepository : IBaseRepository<Nombre>
{ Task<bool> ExistsByXAsync(string x); }

Agregar un método por cada validación contra BD que pidan las reglas.

---

## Command Service - Interface

public interface INombreCommandService
{ Task<Result<Nombre>> Handle(CreateNombreCommand command); }

---

## Command Service - Implementación

El orden dentro del Handle es: primero validaciones simples (sin BD), luego validaciones contra BD, luego el try-catch con save.

Retorno de éxito: return Result<Nombre>.Success(entidad);
Retorno de error: return Result<Nombre>.Failure(NombreBCErrors.ErrorX, "Mensaje");

---

## Repositorio - Implementación

public class NombreRepository(AppDbContext context) : BaseRepository<Nombre>(context), INombreRepository
{ public async Task<bool> ExistsByXAsync(string x) => await Context.Set<Nombre>().AnyAsync(e => e.Campo == x); }

---

## ModelBuilderExtensions

El método se llama ApplyNombreBCConfiguration y se invoca desde AppDbContext en OnModelCreating.

Para propiedades simples: .HasColumnName("nombre_columna").IsRequired().HasMaxLength(90)
Para enums: .HasConversion<int>()
Para owned attributes: entity.OwnsOne(e => e.VoCampo, vo => { vo.Property(v => v.C1).HasColumnName("vo_c1").HasMaxLength(40); });
Siempre agregar CreatedAt y UpdatedAt al final sin IsRequired.

---

## AppDbContext - OnModelCreating

protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    builder.HasDefaultSchema("NombreEsquema");
    builder.ApplyNombreBCConfiguration();
    builder.UseSnakeCaseNamingConvention();
}

---

## Resources

Response (NombreResource): los campos que muestra el API. Enums como int. Excluir Amount u otros si el enunciado lo indica.
Request (CreateNombreResource): los campos que recibe el API. Enums como int.

---

## Assemblers

Entity a Resource: mapea cada propiedad. Enums con cast (int). Owned attributes accediendo a sus subcampos.
Resource a Command: mapea cada campo. Enums con cast (ENombre).
ActionResult: switch de errores a status codes. 400 para errores de negocio. 500 para DatabaseError e InternalServerError. Llama a problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error!, result.Message).

---

## Controller

[ApiController] [Route("api/v1/nombre-entidades")] [Produces("application/json")]
Constructor recibe INombreCommandService y ProblemDetailsFactory.
Método HttpPost recibe [FromBody] CreateNombreResource.
Llama al assembler pasando result, this, lambda de éxito con CreatedAtAction, y problemDetailsFactory.

---

## Program.cs - solo cambia esto

builder.Services.AddScoped<INombreRepository, NombreRepository>();
builder.Services.AddScoped<INombreCommandService, NombreCommandService>();
Y el Description del SwaggerDoc.

---

## appsettings.json - solo cambia esto

"database=NombreDelExamen" en el DefaultConnection.

---

## Migración

dotnet ef migrations add InitialCreate
dotnet ef database update

---

## Checklist rápido

Leer enunciado completo - Identificar BC, aggregate, VOs y reglas
Crear enums y owned attributes - Crear enum de errores
Crear command - Crear aggregate root
Crear IRepository - Crear ICommandService
Crear implementaciones - Crear ModelBuilderExtensions
Actualizar AppDbContext - Crear resources
Crear 3 assemblers - Crear controller
Actualizar Program.cs - Actualizar appsettings
Correr migración - Probar en Swagger
