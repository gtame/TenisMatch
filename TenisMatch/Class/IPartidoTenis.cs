using System;
using System.Collections.Generic;
 
namespace TenisMatch.Class
{

    /// <summary>
    /// Numero de sets un partido
    /// </summary>
    public enum PartidoSets
    {
        None = 0,//Undefined--
        Partido3Sets = 3,
        Partido5Sets = 5
    }


    
    /// <summary>
    /// Interface, definida por si queremos trabajar con DI (Dependency Injection)
    /// </summary>
    public interface IPartidoTenis
    {

        
        /// <summary>
        /// Añade un jugador
        /// </summary>
        /// <param name="jugador">Añade un jugador</param>
        void AddJugador(Jugador jugador);


        /// <summary>
        /// Estado del partido
        /// </summary>
        Estado Estado { get; }


        /// <summary>
        ///  
        ///  
        /// </summary>
        /// <returns>Jugador que realiza el saque</returns>
        Punto ComienzaPunto();


        /// <summary>
        ///  
        ///  
        /// </summary>
        /// <returns>Jugador que realiza el saque</returns>
        JugadorEnum GetJugadorRandom();


        /// <summary>
        /// Añade punto
        /// </summary>
        /// <param name="jugador">Añade un punto al jugador</param>
        void GanaPunto(JugadorEnum jugador);


        /// <summary>
        /// Realiza validaciones antes de comenzar partido
        /// Decide quien saca
        /// </summary>
        /// <returns>Jugador que realiza el saque</returns>
        void ComienzaPartido();


        /// <summary>
        /// Establece el Jugador que realizara el saque
        /// </summary>
        JugadorEnum JugadorSaca { get; set; }


        /// <summary>
        /// Obtiene una enumeracion de los jugadores que participaran en el partido
        /// </summary>
        IReadOnlyList<Jugador> Jugadores { get;  }



        /// <summary>
        /// Obtiene una enumeracion de los sets que se han jugado en el partido
        /// incluido el que actualmente esta en juego
        /// </summary>
        IReadOnlyList<Set> Sets { get; }


        /// <summary>
        /// Establece el numero de sets x partido
        /// </summary>
        PartidoSets NumerodeSets { get; set; }



        /// <summary>
        /// Llamado cuando un set es ganado para chequear si gana el partido.
        /// </summary>
        /// <param name="set"></param>
        void Ganar(Set set);


        #region Callbacks

        #region Comienza
        /// <summary>
        /// Callback comienzo partido
        /// </summary>
        /// <param name="callback"></param>
        void ComienzaPartidoCallback(Action<IPartidoTenis> callback);

        /// <summary>
        /// CallBack comienzo Juego
        /// </summary>
        /// <param name="callback"></param>
        void ComienzaJuegoCallback(Action<Juego> callback);

        /// <summary>
        /// Callback comienzo Set
        /// </summary>
        /// <param name="callback"></param>
        void ComienzaSetCallback(Action<Set> callback);

        
        /// <summary>
        /// Callback comienzo Punto
        /// </summary>
        /// <param name="callback"></param>
        void ComienzaPuntoCallback(Action<Punto> callback);
        #endregion

        #region Gana
        /// <summary>
        /// CallBack Gana Punto
        /// </summary>
        /// <param name="callback"></param>
        void GanaPuntoCallback(Action<Punto> callback);

        /// <summary>
        /// Callback Gana Juego
        /// </summary>
        /// <param name="callback"></param>
        void GanaJuegoCallback(Action<Juego> callback);

        /// <summary>
        /// Callback Gana Set
        /// </summary>
        /// <param name="callback"></param>
        void GanaSetCallback(Action<Set> callback);

        /// <summary>
        /// CallBack gana Partido
        /// </summary>
        /// <param name="callback"></param>
        void GanaPartidoCallback(Action<IPartidoTenis> callback);
        #endregion

        #endregion
    }
}
