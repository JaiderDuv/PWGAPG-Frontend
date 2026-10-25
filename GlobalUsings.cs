// Contratos de API compartidos con el backend.
global using ChefMetrics.Contracts;

// Alias para conservar los nombres históricos usados por las vistas y servicios.
global using InsumoApiDto = ChefMetrics.Contracts.InsumoDto;
global using CrearInsumoRequest = ChefMetrics.Contracts.CrearInsumoDto;
global using ActualizarInsumoRequest = ChefMetrics.Contracts.ActualizarInsumoDto;
global using HistorialPreciosDto = ChefMetrics.Contracts.HistorialPreciosInsumoDto;
global using InsumoOpcionDto = ChefMetrics.Contracts.InsumoDto;
global using ListaInsumosDto = ChefMetrics.Contracts.ListaPaginadaDto<ChefMetrics.Contracts.InsumoDto>;
global using LoginDTO = ChefMetrics.Contracts.LoginDto;
global using ProveedorDTO = ChefMetrics.Contracts.ProveedorDto;
global using ChequeoEliminarProveedorDTO = ChefMetrics.Contracts.ChequeoEliminarProveedorDto;
global using InsumoProveedorDTO = ChefMetrics.Contracts.InsumoProveedorDto;
global using SugerirPrecioRequestDTO = ChefMetrics.Contracts.SugerirPrecioDto;
global using PrecioSugeridoDTO = ChefMetrics.Contracts.PrecioSugeridoDto;
