using System.Text.Json;
using WebConTablas.Models;
namespace WebConTablas.Helpers;
public static class PacienteReporteHelper
{
    public static List<double> SuavizarPromedioMovil(List<int> datos, int ventana = 3)
    {
        var resultado = new List<double>();
        for (int i = 0; i < datos.Count; i++)
        {
            int desde = Math.Max(0, i - ventana + 1);
            int hasta = i;
            var subLista = datos.GetRange(desde, hasta - desde + 1);
            resultado.Add(subLista.Average());
        }
        return resultado;
    }

    public static HashSet<string> ObtenerEmocionesSet(List<DiarioEmocional> diarios)
    {
        var emocionesSet = new HashSet<string>();
        foreach (var d in diarios)
        {
            if (!string.IsNullOrEmpty(d.Emociones))
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(d.Emociones);
                if (dict != null)
                    foreach (var key in dict.Keys)
                        emocionesSet.Add(key);
            }
        }
        return emocionesSet;
    }

    public static Dictionary<string, int> ContarEmociones(List<DiarioEmocional> diarios, HashSet<string> emocionesSet)
    {
        var emocionConteo = new Dictionary<string, int>();
        foreach (var emocion in emocionesSet)
        {
            int count = diarios.Count(d =>
            {
                if (string.IsNullOrEmpty(d.Emociones)) return false;
                var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(d.Emociones);
                return dict != null && dict.ContainsKey(emocion) && dict[emocion] > 0;
            });
            emocionConteo[emocion] = count;
        }
        return emocionConteo;
    }

    public static Dictionary<string, List<int>> ObtenerEmocionesRaw(List<DiarioEmocional> diarios, HashSet<string> emocionesSet)
    {
        var emocionesDataRaw = new Dictionary<string, List<int>>();
        foreach (var emocion in emocionesSet)
        {
            var valores = diarios.Select(d =>
            {
                if (string.IsNullOrEmpty(d.Emociones)) return 0;
                var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(d.Emociones);
                return dict != null && dict.ContainsKey(emocion) ? dict[emocion] : 0;
            }).ToList();

            emocionesDataRaw[emocion] = valores;
        }
        return emocionesDataRaw;
    }

    public static Dictionary<string, List<double>> ObtenerEmocionesSuavizadas(Dictionary<string, List<int>> rawData, int ventana = 3)
    {
        return rawData.ToDictionary(kvp => kvp.Key, kvp => SuavizarPromedioMovil(kvp.Value, ventana));
    }
}
