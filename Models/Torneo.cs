namespace LigaBetplay.Models;

/// <summary>
/// Gestiona equipos en memoria, partidos regulares de liga y tabla de posiciones.
/// </summary>
public sealed class Torneo
{
    private readonly List<Equipo> _equipos = [];

    public IReadOnlyList<Equipo> Equipos => _equipos;

    public bool RegistrarEquipo(string nombre)
    {
        var limpio = nombre.Trim();
        if (string.IsNullOrWhiteSpace(limpio))
            return false;
        if (_equipos.Any(e => string.Equals(e.Nombre, limpio, StringComparison.OrdinalIgnoreCase)))
            return false;
        _equipos.Add(new Equipo(limpio));
        return true;
    }

    public Equipo? BuscarPorNombre(string nombre)
    {
        var limpio = nombre.Trim();
        return _equipos.FirstOrDefault(e =>
            string.Equals(e.Nombre, limpio, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Resultado desde la perspectiva del primer equipo (local).
    /// </summary>
    public bool SimularPartido(Equipo local, Equipo visitante, int golesLocal, int golesVisitante)
    {
        ArgumentNullException.ThrowIfNull(local);
        ArgumentNullException.ThrowIfNull(visitante);
        if (local == visitante)
            return false;
        if (!_equipos.Contains(local) || !_equipos.Contains(visitante))
            return false;
        if (golesLocal < 0 || golesVisitante < 0)
            return false;

        if (golesLocal > golesVisitante)
        {
            local.AplicarPartido(golesLocal, golesVisitante, ResultadoPartido.Ganado);
            visitante.AplicarPartido(golesVisitante, golesLocal, ResultadoPartido.Perdido);
        }
        else if (golesLocal < golesVisitante)
        {
            local.AplicarPartido(golesLocal, golesVisitante, ResultadoPartido.Perdido);
            visitante.AplicarPartido(golesVisitante, golesLocal, ResultadoPartido.Ganado);
        }
        else
        {
            local.AplicarPartido(golesLocal, golesVisitante, ResultadoPartido.Empatado);
            visitante.AplicarPartido(golesVisitante, golesLocal, ResultadoPartido.Empatado);
        }

        return true;
    }

    /// <summary>
    /// Tabla: puntos, diferencia de gol, goles a favor, nombre.
    /// </summary>
    public IReadOnlyList<Equipo> ObtenerTablaOrdenada()
    {
        return _equipos
            .OrderByDescending(e => e.Puntos)
            .ThenByDescending(e => e.DiferenciaGol)
            .ThenByDescending(e => e.GolesFavor)
            .ThenBy(e => e.Nombre, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public Equipo? ObtenerLider() => ObtenerTablaOrdenada().FirstOrDefault();

    public IEnumerable<Equipo> ObtenerTop3() => ObtenerTablaOrdenada().Take(3);

    public IEnumerable<Equipo> EquiposOrdenadosAlfabeticamente() =>
        _equipos.OrderBy(e => e.Nombre, StringComparer.OrdinalIgnoreCase);

    // ——— Consultas LINQ requeridas ———

    public IReadOnlyList<Equipo> EquiposConMasGolesAFavor()
    {
        if (_equipos.Count == 0) return [];
        var m = _equipos.Max(e => e.GolesFavor);
        return _equipos.Where(e => e.GolesFavor == m).ToList();
    }

    public IReadOnlyList<Equipo> EquiposConMenosGolesEnContra()
    {
        if (_equipos.Count == 0) return [];
        var m = _equipos.Min(e => e.GolesContra);
        return _equipos.Where(e => e.GolesContra == m).ToList();
    }

    public IReadOnlyList<Equipo> EquiposConMasVictorias()
    {
        if (_equipos.Count == 0) return [];
        var m = _equipos.Max(e => e.PartidosGanados);
        return _equipos.Where(e => e.PartidosGanados == m).ToList();
    }

    public IReadOnlyList<Equipo> EquiposConMasEmpates()
    {
        if (_equipos.Count == 0) return [];
        var m = _equipos.Max(e => e.PartidosEmpatados);
        return _equipos.Where(e => e.PartidosEmpatados == m).ToList();
    }

    public IReadOnlyList<Equipo> EquiposConMasDerrotas()
    {
        if (_equipos.Count == 0) return [];
        var m = _equipos.Max(e => e.PartidosPerdidos);
        return _equipos.Where(e => e.PartidosPerdidos == m).ToList();
    }

    /// <summary>Invictos / no han perdido: sin derrotas registradas.</summary>
    public IEnumerable<Equipo> EquiposInvictos() =>
        _equipos.Where(e => e.PartidosPerdidos == 0);

    public IEnumerable<Equipo> EquiposSinVictorias() =>
        _equipos.Where(e => e.PartidosGanados == 0);

    public IEnumerable<Equipo> EquiposConDiferenciaPositiva() =>
        _equipos.Where(e => e.DiferenciaGol > 0);

    public IEnumerable<Equipo> EquiposConMasPuntosQue(int puntosMinimos) =>
        _equipos.Where(e => e.Puntos > puntosMinimos);

    public double PromedioGolesAFavorPorEquipo()
    {
        if (_equipos.Count == 0) return 0;
        return _equipos.Average(e => e.GolesFavor);
    }

    public double PromedioGolesEnContraPorEquipo()
    {
        if (_equipos.Count == 0) return 0;
        return _equipos.Average(e => e.GolesContra);
    }

    public int TotalGolesMarcadosTorneo() => _equipos.Sum(e => e.GolesFavor);

    public int TotalPuntosSumados() => _equipos.Sum(e => e.Puntos);

    public double PromedioPuntosPorEquipo()
    {
        if (_equipos.Count == 0) return 0;
        return _equipos.Average(e => e.Puntos);
    }

    public IEnumerable<Equipo> EquiposPorDebajoDelPromedioPuntos()
    {
        if (_equipos.Count == 0)
            return Enumerable.Empty<Equipo>();
        var prom = PromedioPuntosPorEquipo();
        return _equipos.Where(e => e.Puntos < prom);
    }

    /// <summary>Proyección: posición, nombre, puntos y diferencia.</summary>
    public IReadOnlyList<(int Posicion, string Nombre, int Puntos, int DiferenciaGol)> TablaProyeccionPersonalizada()
    {
        return ObtenerTablaOrdenada()
            .Select((e, i) => (i + 1, e.Nombre, e.Puntos, e.DiferenciaGol))
            .ToList();
    }

    /// <summary>
    /// Ranking simple agrupando por puntos (equipos empatados en puntos en el mismo grupo).
    /// </summary>
    public IEnumerable<IGrouping<int, Equipo>> RankingAgrupadoPorPuntos()
    {
        return _equipos
            .GroupBy(e => e.Puntos)
            .OrderByDescending(g => g.Key);
    }

    public string ResumenGeneral()
    {
        var tabla = ObtenerTablaOrdenada();
        var lider = tabla.FirstOrDefault();
        var encuentros = _equipos.Sum(e => e.PartidosJugados) / 2;
        return $"""
            Equipos registrados: {_equipos.Count}
            Partidos jugados (encuentros): {encuentros}
            Goles totales anotados: {TotalGolesMarcadosTorneo()}
            Puntos totales en la tabla: {TotalPuntosSumados()}
            Promedio puntos por equipo: {PromedioPuntosPorEquipo():F2}
            Líder: {lider?.Nombre ?? "—"}
            """;
    }
}
