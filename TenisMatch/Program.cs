using System.Linq;
using System.Text.RegularExpressions;
using TenisMatch.Class;

namespace TenisMatch
{
    class Program
    {
        //Main method..
        static void Main(string[] args)
        {
            Run(new Partido());
        }


        static void Run(IPartidoTenis partido)
        {

            //Solicitar datos user
            #region Inicialización Solicitar datos - Consola

            /*
            partido.AddJugador(new Jugador("Nadal"));
            partido.AddJugador(new Jugador("Djokovic"));
            partido.NumerodeSets = PartidoSets.Partido5Sets;*/

            partido.AddJugador(new Jugador(SolicitarString("Nombre jugador 1: ")));
            partido.AddJugador(new Jugador(SolicitarString("Nombre jugador 2: ")));
            partido.NumerodeSets = (PartidoSets)SolicitarSets("Partido a 3 ó 5 sets? ");

            #endregion

            //Registrar Callbacks - Para mostrar por pantalla desde eventos
            #region Callbacks

            #region Comienza
            ///Callback inicio partido
            partido.ComienzaPartidoCallback(
            (partidoarranca) =>
            {
                Jugador saca = partidoarranca.Jugadores.First(j => j.Id == partidoarranca.JugadorSaca );
                Write(string.Format("{0} iniciará el partido sacando.\r\n",saca.Nombre ));
            });

            ///Callback inicio juego
            partido.ComienzaJuegoCallback(
            (juego) =>
            {
                Write(string.Format("\r\n** Juego {0} - Set {1}", juego.Set.Juegos.Count,juego.Set.Partido.Sets.Count));
            });

   
            #endregion


            #region Gana
            //Ej-**Juego 1 - Set 1
            partido.GanaPuntoCallback(punto =>
            {
                if (punto.PuntuacionTenisJugador1!= PuntuacionJuego.Ganado && punto.PuntuacionTenisJugador2 != PuntuacionJuego.Ganado)
                    Write(punto.ToString(), ((Partido)partido).ToResultString());
            });

            //Callback inicio Juego
            //**Pepo gana el juego 
            partido.GanaJuegoCallback( juego =>
            {
                if (juego.Set.Estado == Estado.EnJuego)
                {
                    Write(juego.ToString(), ((Partido)partido).ToResultString());
                }
            });

            //Callback gana set
            //Supu gana el juego y el set
            partido.GanaSetCallback(set =>
            {
                if (partido.Estado == Estado.EnJuego)
                    Write(set.ToString(), ((Partido)partido).ToResultString());

            });

            //Callback gana el partido
            //!!PEPO GANA EL PARTIDO¡¡  (6-2, 6-7, 6-4)
            partido.GanaPartidoCallback(partidofin =>
            {
          
                Write(partidofin.ToString(), ((Partido)partido).ToResultString());


            });
            #endregion

            #endregion


            //Seleccionamos el jugador que hara el Saque
            partido.JugadorSaca = partido.GetJugadorRandom();

            //Comienza el partido!! 
            partido.ComienzaPartido();

            //Mientras el partido esta en juego 
            //Vamos jugando nuevos puntos
            while (partido.Estado == Estado.EnJuego)
            {
                //Comienza punto
                partido.ComienzaPunto();
                //Obtenemos el jugador  que va a ganar
                JugadorEnum jugadorGanador = partido.GetJugadorRandom();
                //Gana el punto el jugador que seleccionamos        
                partido.GanaPunto(jugadorGanador);
            }

            //Parada para lectura de datos
            System.Console.Read();

        }



        #region Console - Utils
        /// <summary>
        /// Log a consola (Short-cut)
        /// </summary>
        /// <param name="text">Texto a mostrar</param>
        static void Log(string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        /// <summary>
        /// Metodo para escribir texto por consola
        /// </summary>
        /// <param name="text"></param>
        static void Write(string text)
        {
            System.Console.WriteLine(text);
        }

        /// <summary>
        /// Metodo para escribir texto por consola mostrando resultados
        /// </summary>
        /// <param name="text"></param>
        /// <param name="result"></param>
        static void Write(string text, string result)
        {
            System.Console.WriteLine("{0}{1}", text.PadRight(40), result);
        }


        /// <summary>
        /// Solicitar cadena al usuario
        /// </summary>
        /// <param name="texto">Texto a mostrar</param>
        /// <returns></returns>
        static string SolicitarString(string texto)
        {
            return SolicitarInput(texto, @"^[a-zA-Z'.]{1,40}$");
        }

        /// <summary>
        /// Solicitar Sets al usuario
        /// </summary>
        /// <param name="texto">Texto a mostrar</param>
        /// <returns></returns>
        static int SolicitarSets(string texto)
        {
            return int.Parse(SolicitarInput(texto, @"[3,5]"));
        }


        /// <summary>
        /// Solicita la entrada por consola y valida con expresion REgex
        /// </summary>
        /// <param name="texto">Texto a mostrar al usuario</param>
        /// <param name="regex">Patron de validacion</param>
        /// <returns></returns>
        static string SolicitarInput(string texto, string regex)
        {
            bool validate = false;
            string readline;
            do
            {

                Write(texto);
                readline = System.Console.ReadLine();
                //Validaciones aparte...
                Regex reg = new Regex(regex);
                validate = reg.IsMatch(readline);
                if (!validate)
                    Write("Error de validación. Intentalo otra vez");

            } while (!validate);
            return readline;
        }

        #endregion



    }
}
