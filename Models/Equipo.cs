namespace LigaBetplay.Models;

/// <summary>
/// Representa un equipo del torneo con sus estadísticas (PJ derivado de PG+PE+PP).
/// </summary>
public sealed class Equipo
{
    public Equipo(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del equipo no puede estar vacío.", nameof(nombre));
        Nombre = nombre.Trim();
    }

    public string Nombre { get; }

    public int PartidosGanados { get; private set; }
    public int PartidosEmpatados { get; private set; }
    public int PartidosPerdidos { get; private set; }

    public int GolesFavor { get; private set; }
    public int GolesContra { get; private set; }
    public int Puntos { get; private set; }

    /// <summary>PJ: partidos jugados (coherente con PG + PE + PP).</summary>
    public int PartidosJugados => PartidosGanados + PartidosEmpatados + PartidosPerdidos;

    public int DiferenciaGol => GolesFavor - GolesContra;

    internal void AplicarPartido(int golesAFavor, int golesEnContra, ResultadoPartido resultado)
    {
        GolesFavor += golesAFavor;
        GolesContra += golesEnContra;

        switch (resultado)
        {
            case ResultadoPartido.Ganado:
                PartidosGanados++;
                Puntos += 3;
                break;
            case ResultadoPartido.Empatado:
                PartidosEmpatados++;
                Puntos++;
                break;
            case ResultadoPartido.Perdido:
                PartidosPerdidos++;
                break;
        }
    }

    public override string ToString() => Nombre;
}

public enum ResultadoPartido
{
    Ganado,
    Empatado,
    Perdido
}
