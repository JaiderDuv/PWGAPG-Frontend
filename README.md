# ChefMetrics Frontend

Aplicación Blazor de ChefMetrics. Consume la API del repositorio `../ChefMetrics-BackEnd`.

## Requisitos

- .NET SDK 9
- Backend de ChefMetrics en ejecución

## Comandos útiles

```powershell
# Compilar
dotnet build PWGAPG-Frontend.csproj

# Ejecutar en desarrollo
dotnet run --project PWGAPG-Frontend.csproj
```

La URL del backend se configura en `appsettings.json` o en el archivo de configuración del entorno.

## Cambios recientes

- Onboarding reducido a dos pasos: Información Personal e Información del Negocio.
- El onboarding guarda los datos personales y del negocio antes de marcar el proceso como completado.
- Unidades de medida ampliadas en insumos, recetas, productos y proveedores.
- Campo opcional de densidad (`g/ml`) por insumo para convertir tazas/cucharadas a gramos.
- Categorías `Panadería` y `Pastelería` en registro y edición de perfil.

## Documentación

- [Trazabilidad del frontend](docs/trazabilidad.md)
- Matriz completa compartida: `../ChefMetrics-BackEnd/docs/trazabilidad.md`

## Verificación

```powershell
dotnet build PWGAPG-Frontend.csproj
```

Las pruebas automatizadas de los cálculos viven en el repositorio del backend (`ChefMetrics-BackEnd.Tests`).
