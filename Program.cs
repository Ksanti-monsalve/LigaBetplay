using System.Text;
using LigaBetplay.Models;
using LigaBetplay.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var torneo = new Torneo();
bool salir = false;

while (!salir)
{
    MostrarMenuPrincipal();
    var op = Console.ReadLine()?.Trim();
    Console.WriteLine();

    switch (op)
    {
        case "1":
            ListarEquipos(torneo);
            break;
        case "2":
            RegistrarEquipo(torneo);
            break;
        case "3":
            SimularPartido(torneo);
            break;
        case "4":
            VisualizacionTorneo.MostrarTablaPosiciones(torneo);
            break;
        case "5":
            MenuConsultas(torneo);
            break;
        case "6":
            salir = true;
            Console.WriteLine("Hasta luego.");
            break;
        default:
            Console.WriteLine("Opción no válida.");
            break;
    }

    if (!salir)
    {
        Console.WriteLine();
        Console.WriteLine("Pulse Enter para continuar...");
        Console.ReadLine();
    }
}

static void MostrarMenuPrincipal()
{
    Console.Clear();
    Console.WriteLine("=== Simulador Liga BetPlay (consola) ===");
    Console.WriteLine("1. Listar equipos");
    Console.WriteLine("2. Registrar equipos");
    Console.WriteLine("3. Simular partidos");
    Console.WriteLine("4. Ver tabla de posiciones");
    Console.WriteLine("5. Consultar estadísticas del torneo");
    Console.WriteLine("6. Salir del sistema");
    Console.Write("Seleccione una opción: ");
}

static void ListarEquipos(Torneo t)
{
    if (t.Equipos.Count == 0)
    {
        Console.WriteLine("No hay equipos registrados.");
        return;
    }

    Console.WriteLine("Equipos (orden alfabético):");
    foreach (var e in t.EquiposOrdenadosAlfabeticamente())
        Console.WriteLine($"  · {e.Nombre} — PJ:{e.PartidosJugados} PG:{e.PartidosGanados} PE:{e.PartidosEmpatados} PP:{e.PartidosPerdidos} GF:{e.GolesFavor} GC:{e.GolesContra} PTS:{e.Puntos}");
}

static void RegistrarEquipo(Torneo t)
{
    Console.Write("Nombre del equipo: ");
    var nombre = Console.ReadLine() ?? "";
    if (t.RegistrarEquipo(nombre))
        Console.WriteLine($"Equipo «{nombre.Trim()}» registrado.");
    else
        Console.WriteLine("No se pudo registrar (nombre vacío o duplicado).");
}

static void SimularPartido(Torneo t)
{
    if (t.Equipos.Count < 2)
    {
        Console.WriteLine("Se necesitan al menos dos equipos.");
        return;
    }

    MostrarIndiceEquipos(t);
    Console.Write("Índice equipo local: ");
    if (!int.TryParse(Console.ReadLine(), out var iLocal) || iLocal < 1 || iLocal > t.Equipos.Count)
    {
        Console.WriteLine("Índice inválido.");
        return;
    }

    Console.Write("Índice equipo visitante: ");
    if (!int.TryParse(Console.ReadLine(), out var iVis) || iVis < 1 || iVis > t.Equipos.Count)
    {
        Console.WriteLine("Índice inválido.");
        return;
    }

    if (iLocal == iVis)
    {
        Console.WriteLine("Local y visitante deben ser distintos.");
        return;
    }

    var local = t.Equipos[iLocal - 1];
    var vis = t.Equipos[iVis - 1];

    Console.WriteLine("1) Ingresar resultado manualmente  2) Generar aleatorio");
    Console.Write("Opción: ");
    var modo = Console.ReadLine()?.Trim();

    int gl, gv;
    if (modo == "2")
    {
        var rnd = Random.Shared;
        gl = rnd.Next(0, 5);
        gv = rnd.Next(0, 5);
        Console.WriteLine($"Resultado generado: {local.Nombre} {gl} - {gv} {vis.Nombre}");
    }
    else
    {
        Console.Write($"Goles de {local.Nombre}: ");
        if (!int.TryParse(Console.ReadLine(), out gl) || gl < 0)
        {
            Console.WriteLine("Valor inválido.");
            return;
        }

        Console.Write($"Goles de {vis.Nombre}: ");
        if (!int.TryParse(Console.ReadLine(), out gv) || gv < 0)
        {
            Console.WriteLine("Valor inválido.");
            return;
        }
    }

    if (t.SimularPartido(local, vis, gl, gv))
        Console.WriteLine("Partido registrado y estadísticas actualizadas.");
    else
        Console.WriteLine("No se pudo registrar el partido.");
}

static void MostrarIndiceEquipos(Torneo t)
{
    Console.WriteLine("Equipos:");
    for (var i = 0; i < t.Equipos.Count; i++)
        Console.WriteLine($"  {i + 1}. {t.Equipos[i].Nombre}");
}

static void MenuConsultas(Torneo torneo)
{
    bool volver = false;
    while (!volver)
    {
        Console.Clear();
        Console.WriteLine("=== Consultas (LINQ) ===");
        Console.WriteLine("1. Tabla ordenada (puntos, DG, GF, nombre)");
        Console.WriteLine("2. Líder del torneo");
        Console.WriteLine("3. Equipos con más goles a favor");
        Console.WriteLine("4. Equipos con menos goles en contra");
        Console.WriteLine("5. Equipos con más partidos ganados");
        Console.WriteLine("6. Equipos con más empates");
        Console.WriteLine("7. Equipos con más derrotas");
        Console.WriteLine("8. Equipos invictos (sin derrotas)");
        Console.WriteLine("9. Equipos sin victorias");
        Console.WriteLine("10. Top 3 de la tabla");
        Console.WriteLine("11. Equipos con diferencia de gol positiva");
        Console.WriteLine("12. Equipos con más de X puntos");
        Console.WriteLine("13. Buscar equipo por nombre");
        Console.WriteLine("14. Promedio goles a favor (por equipo)");
        Console.WriteLine("15. Promedio goles en contra (por equipo)");
        Console.WriteLine("16. Total goles marcados en el torneo");
        Console.WriteLine("17. Total puntos sumados (todos los equipos)");
        Console.WriteLine("18. Tabla proyección personalizada (pos, nombre, pts, DG)");
        Console.WriteLine("19. Equipos ordenados alfabéticamente");
        Console.WriteLine("20. Resumen general del torneo");
        Console.WriteLine("21. Equipos por debajo del promedio de puntos");
        Console.WriteLine("22. Ranking agrupado por puntos");
        Console.WriteLine("23. Vista: tabla de posiciones completa");
        Console.WriteLine("24. Vista: estadísticas destacadas");
        Console.WriteLine("0. Volver al menú principal");
        Console.Write("Opción: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                VisualizacionTorneo.MostrarTablaPosiciones(torneo);
                break;
            case "2":
                if (torneo.ObtenerLider() is { } lid)
                    Console.WriteLine($"Líder: {lid.Nombre} ({lid.Puntos} pts, DG {lid.DiferenciaGol:+0;-0;0})");
                else
                    Console.WriteLine("No hay equipos.");
                break;
            case "3":
                ImprimirLista("Más goles a favor", torneo.EquiposConMasGolesAFavor());
                break;
            case "4":
                ImprimirLista("Menos goles en contra", torneo.EquiposConMenosGolesEnContra());
                break;
            case "5":
                ImprimirLista("Más victorias", torneo.EquiposConMasVictorias());
                break;
            case "6":
                ImprimirLista("Más empates", torneo.EquiposConMasEmpates());
                break;
            case "7":
                ImprimirLista("Más derrotas", torneo.EquiposConMasDerrotas());
                break;
            case "8":
                ImprimirLista("Invictos (sin derrotas)", torneo.EquiposInvictos().ToList());
                break;
            case "9":
                ImprimirLista("Sin victorias", torneo.EquiposSinVictorias().ToList());
                break;
            case "10":
                ImprimirLista("Top 3", torneo.ObtenerTop3().ToList());
                break;
            case "11":
                ImprimirLista("Diferencia de gol positiva", torneo.EquiposConDiferenciaPositiva().ToList());
                break;
            case "12":
                Console.Write("Puntos mínimos exclusivos (equipos con más que este valor): ");
                if (int.TryParse(Console.ReadLine(), out var min))
                    ImprimirLista($"Más de {min} puntos", torneo.EquiposConMasPuntosQue(min).ToList());
                else
                    Console.WriteLine("Número inválido.");
                break;
            case "13":
                Console.Write("Nombre a buscar: ");
                var bn = Console.ReadLine() ?? "";
                if (torneo.BuscarPorNombre(bn) is { } found)
                    Console.WriteLine($"Encontrado: {found.Nombre} — PJ:{found.PartidosJugados} PTS:{found.Puntos} GF:{found.GolesFavor} GC:{found.GolesContra}");
                else
                    Console.WriteLine("Equipo no encontrado.");
                break;
            case "14":
                Console.WriteLine($"Promedio GF por equipo: {torneo.PromedioGolesAFavorPorEquipo():F2}");
                break;
            case "15":
                Console.WriteLine($"Promedio GC por equipo: {torneo.PromedioGolesEnContraPorEquipo():F2}");
                break;
            case "16":
                Console.WriteLine($"Total goles en el torneo: {torneo.TotalGolesMarcadosTorneo()}");
                break;
            case "17":
                Console.WriteLine($"Suma de puntos de todos los equipos: {torneo.TotalPuntosSumados()}");
                break;
            case "18":
                foreach (var r in torneo.TablaProyeccionPersonalizada())
                    Console.WriteLine($"{r.Posicion,2} | {r.Nombre,-20} | {r.Puntos,3} pts | DG {r.DiferenciaGol:+0;-0;0}");
                break;
            case "19":
                foreach (var e in torneo.EquiposOrdenadosAlfabeticamente())
                    Console.WriteLine($"  · {e.Nombre}");
                break;
            case "20":
                Console.WriteLine(torneo.ResumenGeneral());
                break;
            case "21":
                ImprimirLista("Por debajo del promedio de puntos", torneo.EquiposPorDebajoDelPromedioPuntos().ToList());
                break;
            case "22":
                foreach (var g in torneo.RankingAgrupadoPorPuntos())
                    Console.WriteLine($"{g.Key,3} pts → {string.Join(", ", g.Select(e => e.Nombre))}");
                break;
            case "23":
                VisualizacionTorneo.MostrarTablaPosiciones(torneo);
                break;
            case "24":
                VisualizacionTorneo.MostrarEstadisticasDestacadas(torneo);
                break;
            case "0":
                volver = true;
                continue;
            default:
                Console.WriteLine("Opción no válida.");
                break;
        }

        Console.WriteLine();
        Console.WriteLine("Pulse Enter...");
        Console.ReadLine();
    }
}

static void ImprimirLista(string titulo, IReadOnlyList<Equipo> equipos)
{
    Console.WriteLine(titulo + ":");
    if (equipos.Count == 0)
        Console.WriteLine("  (ninguno)");
    else
        foreach (var e in equipos)
            Console.WriteLine($"  · {e.Nombre}");
}
