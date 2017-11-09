using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenisMatch.Class
{

    /// <summary>
    /// Representa un punto en el partido
    /// </summary>
    /// <remarks>
    /// Como no es clase contenedora hereda de ItemBase no de Item
    /// </remarks>
    public class Punto : ItemBase<Punto>
    {

        #region Properties
        /// <summary>
        /// Puntuacion del juego en 'lenguaje tenis' del player1
        /// </summary>
        public PuntuacionJuego PuntuacionTenisJugador1
        {
            get
            {
                return (PuntuacionJuego)this.PuntosJugador1;
            }
        }



        /// <summary>
        /// Puntuacion del juego en 'lenguaje tenis' del player2
        /// </summary>
        public PuntuacionJuego PuntuacionTenisJugador2
        {
            get
            {
                return (PuntuacionJuego)this.PuntosJugador2;
            }
        }


        /// <summary>
        /// Referencia al Juego donde se esta jugando el punto
        /// </summary>
        public Juego Juego { get; internal set; }


        /// <summary>
        /// Referencia al Set donde se esta jugando el punto
        /// </summary>

        public Set Set
        {
            get
            {
                return Juego.Set;
            }
        }


        /// <summary>
        /// Referencia al Partido donde se esta jugando el punto
        /// </summary>
        public IPartidoTenis Partido
        {
            get
            {
                return Juego.Set.Partido;
            }
        }
        #endregion


        #region Methods

        /// <summary>
        /// Invocado cuando comienza a jugarse es punto
        /// </summary>
        public void Comienza()
        {
            OnComienzo(comienzoCallback);
        }

        /// <summary>
        /// Invocado cuando el punto lo gana un jugador
        /// </summary>
        public void Gana()
        {
            if (Partido == null)
                throw new InvalidOperationException("No hay partido asignado al punto");

            if (JugadorGana == JugadorPierde)
                throw new InvalidOperationException("No se han asignado correctamente el ganador y perdedor del turno");

            //Actualizo puntuacion al Jugador ganador


            PuntuacionJuego puntos1 = PuntuacionTenisJugador1;
            PuntuacionJuego puntos2 = PuntuacionTenisJugador2;
            if (JugadorGana == JugadorEnum.Player1)
                ActualizaMarcador(ref puntos1,ref puntos2);
            else
                ActualizaMarcador(ref puntos2, ref puntos1);

            PuntosJugador1 = (int)puntos1;
            PuntosJugador2 = (int)puntos2;


            OnGana(ganaCallback);
            //Se sobreescribe porque en Punto se llama 'manualmente al evento desde metodo Gana();
            //Y despues en el Juego se le vuelve a pasar el punto
            Estado = Estado.EnJuego;
        }


     


        /// <summary>
        /// Sirve para actualizar el marcador despues de un punto, pasando la puntuacion actual por ref.
        /// </summary>
        /// <param name="puntosGana">Puntos que lleva el que gana el punto</param>
        /// <param name="puntosPierde">Puntos que lleva el que pierde el punto</param>
        private void ActualizaMarcador(ref PuntuacionJuego puntosGana, ref PuntuacionJuego puntosPierde)
        {
            //Si va por debajo de 40
            if (puntosGana <= PuntuacionJuego.Treinta)
                puntosGana++;
            //  -->va en 40 y el otro  tiene 30 o menos
            else if ((puntosGana == PuntuacionJuego.Cuarenta && puntosPierde <= PuntuacionJuego.Treinta)
                //  -->va con Ventaja 
                || (puntosGana == PuntuacionJuego.Ventaja))
            {
                puntosGana = PuntuacionJuego.Ganado;
            }
            //Si van 40 - 40 se pone a ventaja
            else if ((puntosGana == PuntuacionJuego.Cuarenta && puntosPierde == PuntuacionJuego.Cuarenta))
                puntosGana = PuntuacionJuego.Ventaja;
            // Si va el otro jugador con VEntaja se pone el marcador 40 a 40
            else if (puntosPierde == PuntuacionJuego.Ventaja)
            {
                puntosPierde = PuntuacionJuego.Cuarenta;
            }
        }


        #region Override
        /// <summary>
        /// Serializa el punto para mostrar msj. al usuario segun requerimientos
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Estado == Estado.Terminado)
            {
                Jugador ganador = Partido.Jugadores.First(j => j.Id == JugadorGana);
                return string.Format("{0} {1} - {2}", string.Format("Punto de {0} ", ganador.Nombre).PadRight(20), PuntuacionTenisJugador1.ToDescription(), PuntuacionTenisJugador2.ToDescription());
            }
            else
                //Todavia no jugado
                return string.Empty;
        }
        #endregion

        #endregion

    }


    /// <summary>
    /// Contenedor de Puntos 
    /// </summary>
    public class Puntos : Items<Punto> { }
}
