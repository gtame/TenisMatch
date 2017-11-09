using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenisMatch.Class
{
    /// <summary>
    /// Representa cada Set que se juega en el partido
    /// </summary>
    public class Set: Item<Set,Juego,Juegos> 
    {
        #region Constants
        /// <summary>
        /// Constante utilizada para renderizacion de Sets sin inicializar
        /// </summary>
        public static  string ZERO_STRING = "0-0";
        #endregion

        #region Properties
        /// <summary>
        /// Juegos que se han disputado en el SET (o estan en Juego)
        /// </summary>
        public IReadOnlyList<Juego> Juegos { get { return Items.AsReadOnly(); } }


        /// <summary>
        /// Referencia al Partido donde se esta jugando el Set
        /// </summary>
        public IPartidoTenis Partido { get; internal set; }

        /// <summary>
        /// El juejo actualmente en disputa ó NULL si no hay ninguno
        /// </summary>
        public Juego JuegoActual
        {
            get
            {
                return EnJuego;
            }
        }
        #endregion

        #region Methods


        #region Override
        /// <summary>
        /// Invocado cuando un Juego se incia dentro de este Set
        /// </summary>
        /// <param name="item"></param>
        public override void Comenzar(Juego item)
        {
            item.Set = this;
            base.Comenzar(item);
        }

        /// <summary>
        /// Es llamado cada vez que se gana un juego
        /// </summary>
        /// <param name="item">Juego ganado</param>
        public override void Ganar(Juego item)
        {
            //Dependiendo quien gana el juego se incrementa 
            if (item.JugadorGana == JugadorEnum.Player1)
                PuntosJugador1++;
            else
                PuntosJugador2++;

            base.Ganar(item);
        }


        /// <summary>
        /// ES llamado cuando ha ganado un set, llama al metodo Ganar(Set) de la clase partido para chequear si ha ganado tb el partido
        /// PAra ello el metodo HaGanado(Juego item) debe haber devuelto true
        /// </summary>
        /// <param name="action"></param>
        protected override void OnGana(Action<Set> action)
        {
            Partido.Ganar(this);
            base.OnGana(action);
        }


        /// <summar>
        /// Chequeo que determina si al ganar un juego se gana el set
        /// </summary>
        /// <param name="item">Juego que se ha ganado, JugadorGanador cotiene el jugador que lo gano</param>
        /// <returns>true si el Set esta ganado</returns>
        protected override bool HaGanado(Juego item)
        {
            //Tie Break -- Diferencia de 2
            if (PuntosJugador1 >= 5 && PuntosJugador2 >= 5)
            {
                return (Math.Abs(PuntosJugador1 - PuntosJugador2) == 2);
            }
            //Si uno de los dos llega a 6 y el otro no llego a 5
            else if (PuntosJugador1 == 6 || PuntosJugador2 == 6)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Devuelve la cadena cuando el jugador gana segun requerimientos
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Estado == Estado.Terminado)
            {
                Jugador ganador = Partido.Jugadores.First(j => j.Id == JugadorGana);
                return string.Format("{0} gana el juego y el set", ganador.Nombre);
            }
            else
                //Todavia no jugado
                return string.Empty;
        }

        #endregion



        /// <summary>
        /// Imprime el resultado del Set
        /// </summary>
        /// <returns></returns>
        public string ToResultString()
        {
            return string.Format("{0}-{1}", PuntosJugador1, PuntosJugador2);
        }


        #endregion

    }


    /// <summary>
    /// Contenedor de Sets
    /// </summary>
    public class Sets :Items<Set>
    {
    }
}
