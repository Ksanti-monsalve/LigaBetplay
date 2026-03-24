using LigaBetplay.Models;

namespace LigaBetplay.Services;

/// <summary>
/// Presentación en consola de la tabla y métricas destacadas.
/// </summary>
public static class VisualizacionTorneo
{
    /// <summary>Método completo para mostrar la tabla de posiciones con columnas estándar.</summary>
    public static void MostrarTablaPosiciones(Torneo torneo)
    {
        var tabla = torneo.ObtenerTablaOrdenada();
        if (tabla.Count == 0)
        {
            Console.WriteLine("No hay equipos registrados.");
            return;
        }

        const string sep = " | ";
        var rows = tabla
            .Select((e, i) => (
                Pos: (i + 1).ToString(),
                e.Nombre,
                PJ: e.PartidosJugados.ToString(),
                PG: e.PartidosGanados.ToString(),
                PE: e.PartidosEmpatados.ToString(),
                PP: e.PartidosPerdidos.ToString(),
                GF: e.GolesFavor.ToString(),
                GC: e.GolesContra.ToString(),
                DG: e.DiferenciaGol.ToString("+0;-0;0"),
                PTS: e.Puntos.ToString()
            ))
            .ToList();

        int wPos = Math.Max(3, rows.Max(r => r.Pos.Length));
        int wNom = Math.Max(6, rows.Max(r => r.Nombre.Length));
        int wN = 3;

        string H(string t, int w, bool left = false) =>
            left ? t.PadRight(w) : t.PadLeft(w);

        void line(char fill = '═')
        {
            var w = wPos + wNom + 8 * wN + sep.Length * 9 + 2;
            Console.WriteLine(new string(fill, w));
        }

        line('═');
        Console.WriteLine(
            $"{H("POS", wPos)}{sep}{H("Equipo", wNom, true)}{sep}{H("PJ", wN)}{sep}{H("PG", wN)}{sep}{H("PE", wN)}{sep}{H("PP", wN)}{sep}{H("GF", wN)}{sep}{H("GC", wN)}{sep}{H("DG", wN)}{sep}{H("PTS", wN)}");
        line('═');

        foreach (var r in rows)
        {
            Console.WriteLine(
                $"{H(r.Pos, wPos)}{sep}{H(r.Nombre, wNom, true)}{sep}{H(r.PJ, wN)}{sep}{H(r.PG, wN)}{sep}{H(r.PE, wN)}{sep}{H(r.PP, wN)}{sep}{H(r.GF, wN)}{sep}{H(r.GC, wN)}{sep}{H(r.DG, wN)}{sep}{H(r.PTS, wN)}");
        }

        line('═');
    }

    /// <summary>Resumen compacto de líderes y extremos estadísticos.</summary>
    public static void MostrarEstadisticasDestacadas(Torneo torneo)
    {
        if (torneo.Equipos.Count == 0)
        {
            Console.WriteLine("No hay equipos registrados.");
            return;
        }

        void Bloque(string titulo, IEnumerable<Equipo> equipos)
        {
            var list = equipos.ToList();
            Console.WriteLine(titulo);
            if (list.Count == 0)
                Console.WriteLine("  —");
            else
                foreach (var e in list)
                    Console.WriteLine($"  · {e.Nombre}");
            Console.WriteLine();
        }

        Bloque("Líder (criterio de tabla):", torneo.ObtenerLider() is { } l ? [l] : []);
        Bloque("Más goles a favor:", torneo.EquiposConMasGolesAFavor());
        Bloque("Menos goles en contra:", torneo.EquiposConMenosGolesEnContra());
        Bloque("Más partidos ganados:", torneo.EquiposConMasVictorias());
        Bloque("Más empates:", torneo.EquiposConMasEmpates());
        Bloque("Más derrotas:", torneo.EquiposConMasDerrotas());
        Bloque("Invictos (sin derrotas):", torneo.EquiposInvictos());
        Bloque("Sin victorias:", torneo.EquiposSinVictorias());
        Bloque("Top 3:", torneo.ObtenerTop3());
    }
}
