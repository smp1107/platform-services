# 📚 Guía de Reutilización — Práctica Calificada 2

> Basada en el caso Hertz (`platform-services-master`).
> Úsala como referencia para cualquier nuevo examen.

---

## ✅ Lo que NUNCA tocas

### Carpeta `Shared/` — intacta al 100%
```
Shared/
├── Application/
│   ├── Internal/EventHandlers/IEventHandler.cs
│   └── Model/Result.cs                          ← Result<T>
├── Domain/
│   ├── Model/
│   │   ├── Entities/IAuditableEntity.cs          ← herencia del aggregate
│   │   ├── Error.cs                              ← clase base de errores
│   │   └── Events/IEvent.cs
│   └── Repositories/
│       ├── IBaseRepository.cs                    ← herencia del repositorio
│       └── IUnitOfWork.cs
├── Infrastructure/
│   ├── Interfaces/AspNetCore/Configuration/
│   │   ├── KebabCaseRouteNamingConvention.cs     ← kebab-case en URLs
│   │   └── Extensions/StringExtensions.cs
│   └── Persistence/EntityFrameworkCore/
│       ├── Configuration/
│       │   ├── AppDbContext.cs                   ← contexto de BD
│       │   └── Extensions/ModelBuilderExtensions.cs
│       ├── Interceptors/AuditableEntityInterceptor.cs
│       └── Repositories/
│           ├── BaseRepository.cs                 ← implementación base
│           └── UnitOfWork.cs
├── Interfaces/Rest/ProblemDetails/
│   └── ProblemDetailsFactory.cs                  ← manejo de errores HTTP
└── Resources/
    ├── CommonMessages.cs
    └── Errors/ErrorMessage.cs
```

---

## 📝 Lo que SÍ cambias para cada nuevo examen

### 1️⃣ Nombre del Bounded Context
Renombra la carpeta `Services/` con el nombre del BC del enunciado:
```
Services/ → Bookings/ o Payments/ o Reservations/
```

---

### 2️⃣ `Domain/Model/ValueObjects/`

#### Si hay un **enum**:
```csharp
// Hertz tenía EVehicles, tú pones el del enunciado
public enum ECategoria
{
    Valor1 = 1,
    Valor2 = 2,
    Valor3 = 3
}
```
> ⚠️ Siempre asigna los números explícitamente (`= 1`, `= 2`...) si el enunciado los define.

#### Si hay un **owned attribute** (dirección, coordenadas, etc.):
```csharp
public record NombreVO(string Campo1, string Campo2, string Campo3)
{
    public NombreVO() : this(string.Empty, string.Empty, string.Empty) { }
}
```
> ⚠️ El constructor vacío es obligatorio para EF Core.

---

### 3️⃣ `Domain/Model/Errors/`
Crea un enum con los errores específicos del nuevo BC:
```csharp
public enum NuevoBCErrors
{
    None,
    // Agrega los errores según las reglas de negocio del enunciado
    DuplicateX,
    InvalidY,
    OperationCancelled,
    DatabaseError,
    InternalServerError   // siempre al final
}
```
> 💡 Tip: Agrega un error por cada regla de negocio del enunciado.

---

### 4️⃣ `Domain/Model/Commands/`
```csharp
public record CreateNuevaEntidadCommand(
    // solo los parámetros que el usuario envía
    // NO incluir: Id, CreatedAt, UpdatedAt, ni campos calculados
    string Campo1,
    EEnum Campo2,
    double Campo3
);
```

---

### 5️⃣ `Domain/Model/Aggregates/`
```csharp
public class NuevaEntidad : IAuditableEntity
{
    public int Id { get; private set; }
    // ... propiedades del enunciado
    public NuevoVO Campo { get; private set; }     // si hay owned attribute

    public DateTimeOffset? CreatedAt { get; set; } // public set (interceptor)
    public DateTimeOffset? UpdatedAt { get; set; } // public set (interceptor)

    protected NuevaEntidad() { }                   // EF Core

    public NuevaEntidad(CreateNuevaEntidadCommand command)
    {
        // asigna desde el command
    }
}
```

---

### 6️⃣ `Domain/Repositories/`
```csharp
public interface INuevaEntidadRepository : IBaseRepository<NuevaEntidad>
{
    // Agrega métodos según las reglas de negocio
    // Ejemplo: Task<bool> ExistsByXAndYAsync(string x, int y);
}
```

---

### 7️⃣ `Application/CommandServices/`
```csharp
public interface INuevaEntidadCommandService
{
    Task<Result<NuevaEntidad>> Handle(CreateNuevaEntidadCommand command);
}
```

---

### 8️⃣ `Application/Internal/CommandServices/`
```csharp
public class NuevaEntidadCommandService(
    INuevaEntidadRepository repository,
    IUnitOfWork unitOfWork) : INuevaEntidadCommandService
{
    public async Task<Result<NuevaEntidad>> Handle(CreateNuevaEntidadCommand command)
    {
        // Regla 1: validación simple
        if (condicion)
            return Result<NuevaEntidad>.Failure(NuevoBCErrors.ErrorX, "Mensaje");

        // Regla 2: validación contra BD
        var existe = await repository.ExistsByXAsync(command.Campo);
        if (existe)
            return Result<NuevaEntidad>.Failure(NuevoBCErrors.DuplicateX, "Mensaje");

        try
        {
            var entidad = new NuevaEntidad(command);
            await repository.AddAsync(entidad);
            await unitOfWork.CompleteAsync();
            return Result<NuevaEntidad>.Success(entidad);
        }
        catch (Exception ex)
        {
            return Result<NuevaEntidad>.Failure(
                NuevoBCErrors.InternalServerError, ex.Message);
        }
    }
}
```

---

### 9️⃣ `Infrastructure/Persistence/EFC/Repositories/`
```csharp
public class NuevaEntidadRepository(AppDbContext context)
    : BaseRepository<NuevaEntidad>(context), INuevaEntidadRepository
{
    public async Task<bool> ExistsByXAsync(string x)
        => await Context.Set<NuevaEntidad>()
            .AnyAsync(e => e.Campo == x);
}
```

---

### 🔟 `Infrastructure/Persistence/EFC/Configuration/Extensions/`
```csharp
public static class ModelBuilderExtensions
{
    public static void ApplyNuevoBCConfiguration(this ModelBuilder builder)
    {
        builder.Entity<NuevaEntidad>(entity =>
        {
            entity.ToTable("nombre_tabla");           // snake_case
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id").IsRequired().ValueGeneratedOnAdd();

            // propiedades simples
            entity.Property(e => e.Campo1)
                .HasColumnName("campo1").IsRequired().HasMaxLength(90);

            // enum → guardar como int
            entity.Property(e => e.EnumCampo)
                .HasColumnName("enum_campo").IsRequired().HasConversion<int>();

            // owned attribute
            entity.OwnsOne(e => e.DireccionCampo, dir =>
            {
                dir.Property(d => d.Campo1).HasColumnName("dir_campo1")
                    .IsRequired().HasMaxLength(40);
                dir.Property(d => d.Campo2).HasColumnName("dir_campo2")
                    .IsRequired().HasMaxLength(40);
            });

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });
    }
}
```

---

### 1️⃣1️⃣ `Interfaces/REST/Resources/`
```csharp
// Response DTO — lo que retorna el API
public record NuevaEntidadResource(
    int Id,
    string Campo1,
    int EnumCampo,       // enums como int en el response
    double Campo2
    // NO incluir: Amount si el enunciado lo pide omitir
);

// Request DTO — lo que recibe el API
public record CreateNuevaEntidadResource(
    string Campo1,
    int EnumCampo,       // enums como int en el request
    double Campo2
);
```

---

### 1️⃣2️⃣ `Interfaces/REST/Transform/`

#### Assembler Entity → Resource:
```csharp
public static class NuevaEntidadResourceFromEntityAssembler
{
    public static NuevaEntidadResource ToResourceFromEntity(NuevaEntidad entity) =>
        new(entity.Id,
            entity.Campo1,
            (int)entity.EnumCampo,     // cast a int
            entity.NuevoVO.SubCampo);  // si hay owned attribute
}
```

#### Assembler Resource → Command:
```csharp
public static class CreateNuevaEntidadCommandFromResourceAssembler
{
    public static CreateNuevaEntidadCommand ToCommandFromResource(CreateNuevaEntidadResource resource) =>
        new(resource.Campo1,
            (EEnum)resource.EnumCampo, // cast al enum
            resource.Campo2);
}
```

#### Action Result Assembler:
```csharp
public static class NuevaEntidadActionResultAssembler
{
    public static IActionResult ToActionResult(
        Result<NuevaEntidad> result,
        ControllerBase controller,
        Func<NuevaEntidad, IActionResult> onSuccess,
        ProblemDetailsFactory problemDetailsFactory)
    {
        if (result.IsSuccess && result.Value is not null)
            return onSuccess(result.Value);

        var statusCode = (NuevoBCErrors)result.Error! switch
        {
            NuevoBCErrors.InvalidX    => StatusCodes.Status400BadRequest,
            NuevoBCErrors.DuplicateY  => StatusCodes.Status400BadRequest,
            NuevoBCErrors.DatabaseError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        return problemDetailsFactory.CreateProblemDetails(
            controller, statusCode, result.Error!, result.Message);
    }
}
```

---

### 1️⃣3️⃣ `Interfaces/REST/Controller`
```csharp
[ApiController]
[Route("api/v1/nueva-entidades")]
[Produces("application/json")]
[SwaggerTag("Available NuevaEntidad endpoints")]
public class NuevaEntidadController(
    INuevaEntidadCommandService commandService,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Create a nueva entidad", OperationId = "CreateNuevaEntidad")]
    [SwaggerResponse(201, "Created", typeof(NuevaEntidadResource))]
    [SwaggerResponse(400, "Bad Request")]
    [SwaggerResponse(500, "Internal Server Error")]
    public async Task<IActionResult> CreateNuevaEntidad([FromBody] CreateNuevaEntidadResource resource)
    {
        var command = CreateNuevaEntidadCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await commandService.Handle(command);

        return NuevaEntidadActionResultAssembler.ToActionResult(
            result,
            this,
            entidad => CreatedAtAction(
                nameof(CreateNuevaEntidad),
                new { id = entidad.Id },
                NuevaEntidadResourceFromEntityAssembler.ToResourceFromEntity(entidad)),
            problemDetailsFactory);
    }
}
```

---

### 1️⃣4️⃣ `AppDbContext.cs` — agregar en `OnModelCreating`
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    builder.HasDefaultSchema("NombreEsquema"); // ← nombre del examen
    builder.ApplyNuevoBCConfiguration();       // ← tu método
    builder.UseSnakeCaseNamingConvention();
}
```

---

### 1️⃣5️⃣ `Program.cs` — solo cambias estas líneas

```csharp
// Cambia el título
options.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "WebApplication1",
    Version = "v1",
    Description = "NombreExamen API"   // ← esto
});

// Cambia las inyecciones
builder.Services.AddScoped<INuevaEntidadRepository, NuevaEntidadRepository>();
builder.Services.AddScoped<INuevaEntidadCommandService, NuevaEntidadCommandService>();
```

---

### 1️⃣6️⃣ `appsettings.json` — solo el nombre de la BD
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=root;password=;database=NombreExamen"
  }
}
```

---

### 1️⃣7️⃣ Migración
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## ⏱️ Tiempo estimado por sección

| Sección | Tiempo |
|---|---|
| Leer enunciado + identificar VOs y reglas | 5 min |
| Value Objects + Errors enum | 5 min |
| Aggregate Root + Command | 10 min |
| Repository + CommandService | 10 min |
| Infrastructure + ModelBuilder | 5 min |
| Resources + Assemblers + Controller | 10 min |
| Program.cs + migración + prueba Swagger | 5 min |
| **Total** | **~50 min** |

---

## 🎯 Checklist rápido para el examen

```
[ ] Leer enunciado completo
[ ] Identificar: nombre BC, aggregate, VOs, reglas de negocio
[ ] Crear enum(s) de Value Objects
[ ] Crear owned attribute(s) si hay "owned attribute" en el enunciado
[ ] Crear enum de errores (uno por regla de negocio)
[ ] Crear Command
[ ] Crear Aggregate Root
[ ] Crear IRepository con métodos de validación
[ ] Crear ICommandService + implementación con reglas
[ ] Crear Repository con implementación de métodos
[ ] Crear ModelBuilderExtensions con OwnsOne() si hay owned attribute
[ ] Actualizar AppDbContext con HasDefaultSchema y ApplyXConfiguration
[ ] Crear Resources (request y response)
[ ] Crear 3 Assemblers (Entity→Resource, Resource→Command, ActionResult)
[ ] Crear Controller con [HttpPost]
[ ] Actualizar Program.cs (2 AddScoped)
[ ] Actualizar appsettings.json (nombre BD)
[ ] Correr migración
[ ] Probar en Swagger con datos válidos e inválidos
```
