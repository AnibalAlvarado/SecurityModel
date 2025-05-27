# Inclusión Dinámica de Relaciones con Reflection y LINQ en .NET

Este documento explica paso a paso cómo implementar un sistema que permita incluir automáticamente las relaciones de entidades marcadas con una anotación personalizada . Esta funcionalidad se basa en el uso de LINQ, Reflection y atributos personalizados (Attributes).

## ¿Qué problema resuelve?

Normalmente, cuando queremos hacer consultas que incluyan relaciones en Entity Framework, usamos `Include()` de forma manual:

```csharp
context.Ventas.Include(x => x.Cliente).Include(x => x.Producto);
```

Esto funciona, pero tiene dos problemas:

1. Se vuelve repetitivo en cada modelo.
2. Nos obliga a escribir lógica personalizada en cada clase Data si las relaciones cambian.

## ¿Cuál es la idea?

Creamos una anotación (Attribute) que marcará las propiedades del modelo que representan relaciones.

Luego, usamos Reflection para identificar esas propiedades en tiempo de ejecución, construir los Includes automáticamente y devolver un objeto dinámico (ExpandoObject) con solo los campos que nos interesan.

---

## Paso 1: Crear el Attribute personalizado

```csharp
[AttributeUsage(AttributeTargets.Property)]
public class ForeignIncludeAttribute : Attribute
{
    public string? SelectPath { get; set; }
}
```

### Explicación:

* `[AttributeUsage(AttributeTargets.Property)]` indica que este atributo solo se puede usar sobre propiedades.
* `ForeignIncludeAttribute` es una clase que hereda de `Attribute`, lo que la hace un decorador válido.
* `SelectPath` es una propiedad opcional que indica qué campo interno mostrar de la entidad relacionada (ej: `Nombre`).

---

## Paso 2: Crear el helper de Reflection

```csharp
public static class ReflectionHelper
{
    public static object? GetNestedPropertyValue(object obj, string path)
    {
        foreach (var part in path.Split('.'))
        {
            if (obj == null) return null;
            var type = obj.GetType();
            var prop = type.GetProperty(part);
            obj = prop?.GetValue(obj);
        }
        return obj;
    }

    public static string PascalJoin(string a, string b)
    {
        return a + char.ToUpper(b[0]) + b.Substring(1);
    }
}
```

### Explicación:

#### `GetNestedPropertyValue`

* Toma un objeto y un string como "Direccion.Ciudad.Nombre".
* Usa `Split('.')` para separar el camino de propiedades anidadas.
* En cada iteración:

  * Verifica si el objeto actual es `null`. Si lo es, retorna `null`.
  * Obtiene el tipo del objeto actual (`GetType()`).
  * Busca la propiedad actual por nombre (`GetProperty(part)`).
  * Obtiene el valor de esa propiedad y lo asigna al objeto para la siguiente iteración.
* Al finalizar, retorna el valor final encontrado.

#### `PascalJoin`

* Une dos strings respetando PascalCase, útil para formar nombres de propiedades en el resultado final.
* Por ejemplo: `Cliente` y `Nombre` → `ClienteNombre`.

---

## Paso 3: Lógica en la capa Data

```csharp
public virtual async Task<List<ExpandoObject>> GetAllDynamicAsync()
{
    var entityType = typeof(T); // Tipo del modelo genérico T

    var query = _db.Set<T>().AsQueryable(); // Se obtiene el DbSet y se convierte en IQueryable

    // Buscar propiedades con el atributo personalizado
    var foreignKeyProps = entityType
        .GetProperties() // Obtener todas las propiedades del modelo
        .Where(p => Attribute.IsDefined(p, typeof(ForeignIncludeAttribute))) // Filtrar por las que tienen el atributo
        .ToList();

    // Se agregan los Includes dinámicamente
    foreach (var prop in foreignKeyProps)
    {
        query = query.Include(prop.Name); // Incluir la propiedad relacionada
    }

    var resultList = await query.ToListAsync(); // Ejecutar la consulta con EF y traer los datos
    var dynamicList = new List<ExpandoObject>(); // Lista que almacenará los objetos dinámicos

    // Se recorre cada entidad del resultado
    foreach (var entity in resultList)
    {
        dynamic dyn = new ExpandoObject(); // Se crea un objeto dinámico
        var dict = (IDictionary<string, object?>)dyn; // Se accede como diccionario para agregar propiedades

        dict["Id"] = entityType.GetProperty("Id")?.GetValue(entity); // Se obtiene el Id del objeto

        // Se recorre cada propiedad con ForeignIncludeAttribute
        foreach (var prop in foreignKeyProps)
        {
            var attr = prop.GetCustomAttribute<ForeignIncludeAttribute>()!; // Obtener el atributo
            var foreignValue = prop.GetValue(entity); // Obtener la entidad relacionada
            if (foreignValue == null) continue; // Saltar si es null

            if (!string.IsNullOrEmpty(attr.SelectPath))
            {
                // Si se especificó un campo interno, obtenerlo dinámicamente
                var value = ReflectionHelper.GetNestedPropertyValue(foreignValue, attr.SelectPath);
                var key = ReflectionHelper.PascalJoin(prop.Name, attr.SelectPath); // Crear nombre de propiedad compuesto
                dict[key] = value; // Agregar al objeto dinámico
            }
            else
            {
                // Si no se especificó SelectPath, incluir el objeto completo
                dict[prop.Name] = foreignValue;
            }
        }

        dynamicList.Add(dyn); // Agregar a la lista de resultados
    }

    return dynamicList; // Devolver la lista final de objetos dinámicos
}
```

---

## Paso 4: En la capa Business

### En IBaseModelBusiness.cs:

```csharp
/// <summary>
/// Obtener listado con relaciones dinámicas
/// </summary>
/// <returns>Lista de ExpandoObject</returns>
Task<List<ExpandoObject>> GetAllDynamicAsync();
```

### En ABaseModelBusiness.cs:

```csharp
/// <summary>
/// Obtener listado con relaciones dinámicas (passthrough)
/// </summary>
/// <returns>Lista de ExpandoObject</returns>
public virtual async Task<List<ExpandoObject>> GetAllDynamicAsync()
{
    return await _data.GetAllDynamicAsync();
}
```

---

## Paso 5: Consumir en el Controller

```csharp
[HttpGet("dynamic")]
public async Task<IActionResult> GetDynamicAsync()
{
    var result = await _ventaBusiness.GetAllDynamicAsync();
    return Ok(result);
}
```

---

## Definiciones importantes

 **Reflection**: Es una característica de .NET que permite inspeccionar tipos, atributos y propiedades de los objetos en tiempo de ejecución.
 **Attribute**: Son metadatos que se pueden aplicar a clases, propiedades o métodos para agregar información adicional.

 **ExpandoObject**: Es una estructura de datos que permite agregar propiedades dinámicamente como si fuera un diccionario flexible.

 **Include (LINQ)**: Permite a Entity Framework incluir relaciones (joins) en la consulta antes de ejecutarla, evitando lazy loading.

ExpandoObject es una clase especial que permite agregar o quitar propiedades dinámicamente en tiempo de ejecución.

Al declararlo como dynamic, puedes usar dyn.Propiedad = valor; sin que el compilador lo valide en tiempo de compilación.

---

## Ventajas

 Evita repetir Includes en cada Data concreta.
 Permite seleccionar solo campos específicos de las relaciones.
 Soporta relaciones anidadas.
 Devuelve un JSON limpio y compacto.

---

¡Con esta estructura puedes mantener tus consultas limpias, reutilizables y totalmente automáticas! Si quieres, puedo ayudarte a agregar filtros o paginación sobre estos resultados.
