using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
 
namespace TenisMatch.Class
{

        /// <summary>
        /// Enumeración de los puntos de tenis 
        /// </summary>
       public enum PuntuacionJuego
        {
            [Description("0")]
            Cero=0,
            [Description("15")]
            Quince=1,
            [Description("30")]
            Treinta=2,
            [Description("40")]
            Cuarenta=3,
            [Description("Ad")]
            Ventaja=98,
            [Description("Win")]
            Ganado=99,
    }


    /// <summary>
    /// Represanta a un Juego dentro de un Set
    /// </summary>
    public class Juego : Item<Juego, Punto,Puntos> 
    {
        #region properties

        /// <summary>
        /// Puntos jugados en ese Juego incluido el que se esta jugando
        /// </summary>
        public IReadOnlyList<Punto> Puntos { get { return Items.AsReadOnly(); } }
        
        /// <summary>
        /// Puntuacion del juego en 'lenguaje tenis' del player1
        /// </summary>
        public PuntuacionJuego PuntuacionTenisJugador1
        {
            get
            {
                return (PuntuacionJuego)PuntosJugador1;
            }
        }


        /// <summary>
        /// Puntuacion del juego en 'lenguaje tenis' del player2
        /// </summary>
        public PuntuacionJuego PuntuacionTenisJugador2
        {
            get
            {
                    
                return (PuntuacionJuego)PuntosJugador2;
            }
        }


        /// <summary>
        /// El punto actualmente en juego ó NULL si no hay ninguno
        /// </summary>
        public Punto PuntoActual
        {
            get
            {
                return EnJuego;
            }
        }

        /// <summary>
        /// El set al que pertenece el juego
        /// </summary>
        public Set Set { get; internal set; }


        /// <summary>
        /// El partido al que pertenece el juego - set
        /// </summary>
        public IPartidoTenis Partido { get { return Set.Partido; } }

        #endregion


        #region overrides
        /// <summary>
        /// Es llamado cuando se comienza un punto en ese juego
        /// </summary>
        /// <param name="item">Punto que comienza</param>
        public override void Comenzar(Punto item)
        {
            //Fijamos juego del punto
            item.Juego = this;
            //Fijamos los marcadores actuales del juego
            item.PuntosJugador1 = PuntosJugador1;
            item.PuntosJugador2 = PuntosJugador2;
            //Al metodo base para que lo añada a la coleccion
            base.Comenzar(item);
            //Metodo comienza del punto para que lance eventos
            item.Comienza();
        }

        /// <summary>
        /// Es llamado cuando se gana el punto que esta en juego 
        /// el jugador Ganador / Perdedor ya deberia estar asignado
        /// </summary>
        /// <param name="item"></param>
        public override void Ganar(Punto item)
        {
            //Ya debe estar asignado el JugadorGanador-JugadorPerdedor
            //Aqui se actualiza el marcador del punto
            item.Gana();

            //Actualizamos el marcador del juego con el del punto actual
            PuntosJugador1 = item.PuntosJugador1;
            PuntosJugador2 = item.PuntosJugador2;

            base.Ganar(item);

            //Ya no esta en juego ese punto
            EnJuego = null;

        }


        /// <summary>
        /// Es llamado para invocar delegados e informar del echo al objeto padre (Set)
        /// </summary>
        /// <param name="action"></param>
        protected override void OnGana(Action<Juego> action)
        {
            Set.Ganar(this);
            base.OnGana(action);
            

        }

        /// <summary>
        /// Cuando se gana un punto, se chequea si el Juego esta terminado
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool HaGanado(Punto item)
        {
            //Si alguno ha llegado al marcador 'ganado'
            if (
                (PuntuacionTenisJugador1 == PuntuacionJuego.Ganado) ||
                (PuntuacionTenisJugador2 == PuntuacionJuego.Ganado)
                )
            {
                return true;
            }
            else
                return false;
        }



        /// <summary>
        /// Serializa a string segun requerimientos
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Estado == Estado.Terminado)
            {
                Jugador ganador = Partido.Jugadores.First(j => j.Id == JugadorGana);
                return string.Format("{0} gana el juego", ganador.Nombre);
            }
            else
                //Esta en juego
                return string.Empty;
        }
        #endregion 
    }


    /// <summary>
    /// Coleccion de juegos
    /// </summary>
    public class Juegos:Items<Juego>
    {
    }
}
