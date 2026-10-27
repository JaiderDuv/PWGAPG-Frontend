# Trazabilidad del frontend

Rutas relativas a `C:\PGC`. Esta matriz complementa la del backend (`../ChefMetrics-BackEnd/docs/trazabilidad.md`) y se centra en lo que el usuario ve y envía.

| Propuesta / necesidad | Funcionalidad en la interfaz | Archivos principales | Verificación |
|---|---|---|---|
| Usar las unidades reales de una panadería o pastelería | Los selectores de unidad de insumos, recetas, productos y proveedores se alimentan del catálogo compartido `ConversorUnidades.Unidades` (mg, g, kg, oz, lb, ml, l, cdta, cda, taza, floz, pt, qt, gal, unidad, docena). | `Components/RecetaForm.razor`; `Components/ProductoMenuForm.razor`; `Pages/Insumos.razor`; `Pages/InsumosProveedores.razor`; `Pages/ProveedoresInsumo.razor`; `Models/MenuModels.cs`; `Models/ProveedorDTO.cs` | Compilación del frontend; pruebas de conversión en `ChefMetrics-BackEnd.Tests/ConversorUnidadesTests.cs`. |
| Convertir una taza de harina a gramos | El alta/edición de insumos permite una densidad opcional en g/ml. Las recetas muestran solo unidades compatibles y calculan el costo de la línea usando esa densidad. | `Pages/Insumos.razor`; `Components/RecetaForm.razor`; `Models/ProveedorDTO.cs` | Compilación; pruebas de densidad y costo en `ConversorUnidadesTests`, `CostoRecetaServiceTests` y `CostoProductoServiceTests`. |
| Cerrar el onboarding incompleto | El onboarding queda en dos pestañas. La primera guarda datos personales con `POST api/onboarding/datos-personales`; la segunda guarda el negocio con `POST api/onboarding/datos-negocio` y luego llama a `completar`. Se retiraron Empleados, Productos e Insumos. | `Pages/Onboarding.razor`; `Pages/OnboardingTabs/InformacionPersonalTab.razor`; `Pages/OnboardingTabs/InformacionNegocioTab.razor`; `Models/DuenioDTO.cs`; `Models/OnboardingNegocioDto.cs` | Compilación; prueba manual del flujo de dos pasos contra el backend. |
| Evitar que un negocio formalizado quede bloqueado por el documento personal | El documento personal es opcional en el DTO del onboarding; el backend lo exige solo para negocios no formalizados y usa NIT/Razón Social/RUES para los formalizados. | `Models/DuenioDTO.cs`; `Pages/OnboardingTabs/InformacionPersonalTab.razor`; `ChefMetrics-BackEnd/Controllers/OnboardingController.cs` | Compilación; prueba manual con un usuario formalizado. |
| Categorías alineadas al público objetivo | Registro y edición de perfil muestran Panadería, Pastelería, Repostería, Cafetería, Gastronomía, Productos frescos y Otros. | `Pages/Registro.razor`; `Pages/EditarPerfilEmprendimiento.razor` | Verificación manual en ambos formularios. |
| Costos visibles coherentes con el backend | El formulario de receta calcula el costo de cada fila con conversión, merma y densidad; el formulario de producto usa las mismas unidades para empaques. | `Components/RecetaForm.razor`; `Components/ProductoMenuForm.razor` | Compilación; pruebas de backend `CalculadoraCostosTests`, `CostoRecetaServiceTests` y `CostoProductoServiceTests`. |
| Documentar el cambio | README del frontend y esta matriz. | `README.md`; `docs/trazabilidad.md` | Revisión manual. |

## Verificación usada

```powershell
cd C:\PGC\PWGAPG-Frontend
dotnet build PWGAPG-Frontend.csproj
```

Resultado: compilación correcta, 0 errores y 0 advertencias.

Las pruebas automatizadas de los cálculos se ejecutan en el backend:

```powershell
cd C:\PGC\ChefMetrics-BackEnd
dotnet test ChefMetrics-BackEnd.Tests\ChefMetrics-BackEnd.Tests.csproj
```

Resultado: 65 pruebas correctas.
